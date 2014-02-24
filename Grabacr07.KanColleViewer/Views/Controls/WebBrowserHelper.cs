using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using SHDocVw;
using IServiceProvider = Grabacr07.KanColleViewer.Win32.IServiceProvider;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	public class WebBrowserHelper
	{
		#region ScriptErrorsSuppressed 添付プロパティ

		public static readonly DependencyProperty ScriptErrorsSuppressedProperty =
			DependencyProperty.RegisterAttached("ScriptErrorsSuppressed", typeof(bool), typeof(WebBrowserHelper), new PropertyMetadata(default(bool), ScriptErrorsSuppressedChangedCallback));

		public static void SetScriptErrorsSuppressed(WebBrowser browser, bool value)
		{
			browser.SetValue(ScriptErrorsSuppressedProperty, value);
		}

		public static bool GetScriptErrorsSuppressed(WebBrowser browser)
		{
			return (bool)browser.GetValue(ScriptErrorsSuppressedProperty);
		}

		private static void ScriptErrorsSuppressedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var browser = d as WebBrowser;
			if (browser == null) return;
			if (!(e.NewValue is bool)) return;

			try
			{
				var axIWebBrowser2 = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
				if (axIWebBrowser2 == null) return;

				var comObj = axIWebBrowser2.GetValue(browser, null);
				if (comObj == null) return;

				comObj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, comObj, new[] { e.NewValue, });
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		#endregion

		#region ZoomFactor 添付プロパティ

		public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.RegisterAttached(
			"ZoomFactor", typeof(int), typeof(WebBrowserHelper), new PropertyMetadata(100, ZoomFactorChangedCallback));

		public static void SetZoomFactor(WebBrowser browser, int value)
		{
			browser.SetValue(ZoomFactorProperty, value);
		}

		public static int GetZoomFactor(WebBrowser browser)
		{
			return (int)browser.GetValue(ZoomFactorProperty);
		}

		private static void ZoomFactorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var browser = d as WebBrowser;
			if (browser == null) return;
			if (!(e.NewValue is int)) return;

			var zoomFactor = (int)e.NewValue;

			if (zoomFactor < 10 || zoomFactor > 1000)
			{
				StatusService.Current.Notify(string.Format(Resources.ZoomAction_OutOfRange, zoomFactor));
				return;
			}

			try
			{
				var provider = browser.Document as IServiceProvider;
				if (provider == null) return;

				object ppvObject;
				provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
				var webBrowser = ppvObject as IWebBrowser2;
				if (webBrowser == null) return;

				object pvaIn = zoomFactor;
				webBrowser.ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref pvaIn);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				StatusService.Current.Notify(string.Format(Resources.ZoomAction_ZoomFailed, ex.Message));
			}
		}

		#endregion
	}
}
