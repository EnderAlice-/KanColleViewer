using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleViewer.Win32;
using Grabacr07.KanColleWrapper;
using Livet;
using MetroRadiance;
using AppSettings = Grabacr07.KanColleViewer.Properties.Settings;
using Settings = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer
{
	public partial class App
	{
		public static ProductInfo ProductInfo { get; private set; }
		public static MainWindowViewModel ViewModelRoot { get; private set; }

		static App()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this.DispatcherUnhandledException += (sender, args) => ReportException(sender, args.Exception);

			#region アフィニティ マスクの変更
			try
			{
				var myThread = Kernel32.GetCurrentThread();

				var processorNumber = new PROCESSOR_NUMBER(0, 0);
				if(Kernel32.GetThreadIdealProcessorEx(myThread, ref processorNumber))
				{
					var groupAffinity = new GROUP_AFFINITY(processorNumber.Group, (UIntPtr)(1 << processorNumber.Number));
					Kernel32.SetThreadGroupAffinity(myThread, ref groupAffinity);

#if _DEBUG
					//MessageBox.Show("アフィニティマスクを変更しました。\nプロセッサ グループ:" + processorNumber.Group + "\nプロセッサ番号:" + processorNumber.Number, "デバッグ", MessageBoxButton.OK, MessageBoxImage.Information);
#endif
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			#endregion

			DispatcherHelper.UIDispatcher = this.Dispatcher;
			ProductInfo = new ProductInfo();

			Settings.Load();
			WindowsNotification.Notifier.Initialize();
			Helper.SetRegistryFeatureBrowserEmulation();

			KanColleClient.Current.Proxy.Startup(AppSettings.Default.LocalProxyPort);
			KanColleClient.Current.Proxy.UseProxyOnConnect = Settings.Current.EnableProxy;
			KanColleClient.Current.Proxy.UseProxyOnSSLConnect = Settings.Current.EnableSSLProxy;
			KanColleClient.Current.Proxy.UpstreamProxyHost = Settings.Current.ProxyHost;
			KanColleClient.Current.Proxy.UpstreamProxyPort = Settings.Current.ProxyPort;

			ResourceService.Current.ChangeCulture(Settings.Current.Culture);
			KanColleClient.Current.Logger.EnableLogging = Settings.Current.EnableLogging;

			ThemeService.Current.Initialize(this, Theme.Dark, Accent.Purple);

			ViewModelRoot = new MainWindowViewModel();
			this.MainWindow = new MainWindow { DataContext = ViewModelRoot };
			this.MainWindow.Show();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			KanColleClient.Current.Proxy.Shutdown();

			WindowsNotification.Notifier.Dispose();
			Settings.Current.Save();
		}


		private static void ReportException(object sender, Exception exception)
		{
			#region const
			const string messageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
{2}
";
			const string path = "error.log";
			#endregion

			try
			{
				var message = string.Format(messageFormat, DateTimeOffset.Now, sender, exception);

				Debug.WriteLine(message);
				File.AppendAllText(path, message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
