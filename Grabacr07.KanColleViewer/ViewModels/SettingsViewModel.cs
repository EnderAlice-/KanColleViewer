﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using MetroRadiance;
using Settings = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class SettingsViewModel : TabItemViewModel, INotifyDataErrorInfo
	{
		public override string Name
		{
			get { return Resources.Settings; }
			protected set { throw new NotImplementedException(); }
		}

		#region ScreenshotFolder 変更通知プロパティ

		public string ScreenshotFolder
		{
			get { return Settings.Current.ScreenshotFolder; }
			set
			{
				if (Settings.Current.ScreenshotFolder != value)
				{
					Settings.Current.ScreenshotFolder = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("CanOpenScreenshotFolder");
				}
			}
		}

		#endregion

		#region CanOpenScreenshotFolder 変更通知プロパティ

		public bool CanOpenScreenshotFolder
		{
			get { return Directory.Exists(this.ScreenshotFolder); }
		}

		#endregion

		#region ScreenshotImageFormat 変更通知プロパティ

		public SupportedImageFormat ScreenshotImageFormat
		{
			get { return Settings.Current.ScreenshotImageFormat; }
			set
			{
				if (Settings.Current.ScreenshotImageFormat != value)
				{
					Settings.Current.ScreenshotImageFormat = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ProxyPort 変更通知プロパティ

		public string ProxyPort
		{
			get { return Settings.Current.ProxySettings.Port.ToString(CultureInfo.InvariantCulture); }
			set
			{
				ushort port;
				if (ushort.TryParse(value, out port) && Settings.Current.ProxySettings.Port != port)
				{
					Settings.Current.ProxySettings.Port = port;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ReSortieCondition 変更通知プロパティ

		private string _ReSortieCondition = Settings.Current.ReSortieCondition.ToString(CultureInfo.InvariantCulture);
		private string reSortieConditionError;

		public string ReSortieCondition
		{
			get { return this._ReSortieCondition; }
			set
			{
				if (this._ReSortieCondition != value)
				{
					ushort cond;
					if (ushort.TryParse(value, out cond) && cond <= 49)
					{
						Settings.Current.ReSortieCondition = cond;
						this.reSortieConditionError = null;
					}
					else
					{
						this.reSortieConditionError = "コンディション値は 0 ～ 49 の数値で入力してください。";
					}

					this._ReSortieCondition = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Libraries 変更通知プロパティ

		private IEnumerable<BindableTextViewModel> _Libraries;

		public IEnumerable<BindableTextViewModel> Libraries
		{
			get { return this._Libraries; }
			set
			{
				if (!Equals(this._Libraries, value))
				{
					this._Libraries = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ColorTheme 変更通知プロパティ

		public Theme ColorTheme
		{
			get { return Settings.Current.ColorTheme; }
			set
			{
				if (Settings.Current.ColorTheme != value)
				{
					Settings.Current.ColorTheme = value;
					this.RaisePropertyChanged();
					ThemeService.Current.ChangeTheme(value);
				}
			}
		}

		#endregion

		#region Cultures 変更通知プロパティ

		private IReadOnlyCollection<CultureViewModel> _Cultures;

		public IReadOnlyCollection<CultureViewModel> Cultures
		{
			get { return this._Cultures; }
			set
			{
				if (!Equals(this._Cultures, value))
				{
					this._Cultures = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Culture 変更通知プロパティ

		/// <summary>
		/// カルチャを取得または設定します。
		/// </summary>
		public string Culture
		{
			get { return Settings.Current.Culture; }
			set
			{
				if (Settings.Current.Culture != value)
				{
					ResourceService.Current.ChangeCulture(value);
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BrowserZoomFactor 変更通知プロパティ

		private BrowserZoomFactor _BrowserZoomFactor;

		public BrowserZoomFactor BrowserZoomFactor
		{
			get { return this._BrowserZoomFactor; }
			private set
			{
				if (this._BrowserZoomFactor != value)
				{
					this._BrowserZoomFactor = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableLogging 変更通知プロパティ

		public bool EnableLogging
		{
			get { return Settings.Current.EnableLogging; }
			set
			{
				if (Settings.Current.EnableLogging != value)
				{
					Settings.Current.EnableLogging = value;
					KanColleClient.Current.Homeport.Logger.EnableLogging = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion



		public bool HasErrors
		{
			get { return this.reSortieConditionError != null; }
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


		public SettingsViewModel()
		{
			if (Helper.IsInDesignMode) return;

			this.Libraries = App.ProductInfo.Libraries.Aggregate(
				new List<BindableTextViewModel>(),
				(list, lib) =>
				{
					list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", " });
					list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
					// プロダクト名の途中で改行されないように、space を non-break space に置き換えてあげてるんだからねっっ
					return list;
				});

			this.Cultures = new[] { new CultureViewModel { DisplayName = "(auto)" } }
				.Concat(ResourceService.Current.SupportedCultures
					.Select(x => new CultureViewModel { DisplayName = x.EnglishName, Name = x.Name })
					.OrderBy(x => x.DisplayName))
				.ToList();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(Settings.Current)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});

			ThemeService.Current.ChangeTheme(Settings.Current.ColorTheme);

			var zoomFactor = new BrowserZoomFactor { Current = Settings.Current.BrowserZoomFactor };
			this.CompositeDisposable.Add(new PropertyChangedEventListener(zoomFactor)
			{
				{ "Current", (sender, args) => Settings.Current.BrowserZoomFactor = zoomFactor.Current },
			});
			this.BrowserZoomFactor = zoomFactor;
		}


		public void OpenScreenshotFolderSelectionDialog()
		{
			var message = new FolderSelectionMessage("OpenFolderDialog/Screenshot")
			{
				Title = Resources.Settings_Screenshot_FolderSelectionDialog_Title,
				DialogPreference = Helper.IsWindows8OrGreater
					? FolderSelectionDialogPreference.CommonItemDialog
					: FolderSelectionDialogPreference.FolderBrowser,
				SelectedPath = this.CanOpenScreenshotFolder
					? this.ScreenshotFolder
					: ""
			};
			this.Messenger.Raise(message);

			if (Directory.Exists(message.Response))
			{
				this.ScreenshotFolder = message.Response;
			}
		}

		public void OpenScreenshotFolder()
		{
			if (this.CanOpenScreenshotFolder)
			{
				try
				{
					Process.Start(this.ScreenshotFolder);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
		}

		public void ClearZoomFactor()
		{
			App.ViewModelRoot.Messenger.Raise(new InteractionMessage { MessageKey = "WebBrowser/Zoom" });
		}

		public void SetLocationLeft()
		{
			App.ViewModelRoot.Messenger.Raise(new SetWindowLocationMessage { MessageKey = "Window/Location", Left = 0.0 });
		}


		public IEnumerable GetErrors(string propertyName)
		{
			var errors = new List<string>();

			switch (propertyName)
			{
				case "ReSortieCondition":
					if (this.reSortieConditionError != null)
					{
						errors.Add(this.reSortieConditionError);
					}
					break;
			}

			return errors.HasItems() ? errors : null;
		}

		protected void RaiseErrorsChanged([CallerMemberName]string propertyName = "")
		{
			if (this.ErrorsChanged != null)
			{
				this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
			}
		}
	}
}
