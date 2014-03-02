using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grabacr07.KanColleViewer.Models;
using Livet;
using Livet.EventListeners;
using MetroRadiance.Core;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	/// <summary>
	/// ZoomFactorSelector.xaml の相互作用ロジック
	/// </summary>
	public partial class ZoomFactorSelector
	{
		private List<ZoomFactorSelectorItem> items;
		private IDisposable zoomFactorNotifyListener;

		internal class ZoomFactorSelectorItem : NotificationObject
		{
			public Action SelectAction { get; set; }
			public bool IsDefault { get; set; }

			#region Value 変更通知プロパティ

			private int _Value;

			public int Value
			{
				get { return this._Value; }
				set
				{
					if (this._Value != value)
					{
						this._Value = value;
						this.RaisePropertyChanged();
					}
				}
			}

			#endregion

			#region IsSelected 変更通知プロパティ

			private bool _IsSelected;

			public bool IsSelected
			{
				get { return this._IsSelected; }
				set
				{
					if (this._IsSelected != value)
					{
						this._IsSelected = value;
						this.RaisePropertyChanged();
					}
				}
			}

			#endregion

			public void Select()
			{
				if (this.SelectAction != null) this.SelectAction();
			}
		}


		#region ZoomFactor 依存関係プロパティ

		public IZoomFactor ZoomFactor
		{
			get { return (IZoomFactor)this.GetValue(ZoomFactorProperty); }
			set { this.SetValue(ZoomFactorProperty, value); }
		}
		public static readonly DependencyProperty ZoomFactorProperty =
			DependencyProperty.Register("ZoomFactor", typeof(IZoomFactor), typeof(ZoomFactorSelector), new UIPropertyMetadata(null, ZoomFactorPropertyChangedCallback));

		private static void ZoomFactorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (ZoomFactorSelector)d;
			var newValue = (IZoomFactor)e.NewValue;

			if (source.zoomFactorNotifyListener != null)
			{
				source.zoomFactorNotifyListener.Dispose();
			}

			if (newValue != null)
			{
				var dpi = source.GetSystemDpi();
				source.items = newValue.SupportedValues
					.Select(x => new ZoomFactorSelectorItem
					{
						Value = (int)(x * 100),
						IsSelected = x.Equals(newValue.Current),
						SelectAction = () => newValue.Current = x,
						IsDefault = dpi.HasValue && dpi.Value.ScaleX.Equals(x),
					})
					.ToList();
				source.SupportedList.ItemsSource = source.items;
			}

			var notifySource = newValue as INotifyPropertyChanged;
			if (notifySource != null)
			{
				source.zoomFactorNotifyListener = new PropertyChangedEventListener(notifySource)
				{
					{
						"Current",
						(sender, args) =>
						{
							var target = source.items.FirstOrDefault(x => x.Value == (int)(newValue.Current * 100));
							if (target != null) target.IsSelected = true;
						}
					}
				};
			}
		}

		#endregion


		public ZoomFactorSelector()
		{
			InitializeComponent();

			this.Popup.Opened += (sender, args) => this.ChangeBackground();
			this.Popup.Closed += (sender, args) => this.ChangeBackground();
		}


		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			this.ChangeBackground();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			this.ChangeBackground();
		}


		private void ZoomDown(object sender, RoutedEventArgs e)
		{
			if (this.ZoomFactor != null && this.ZoomFactor.CanZoomDown)
			{
				this.ZoomFactor.ZoomDown();
			}
		}

		private void ZoomUp(object sender, RoutedEventArgs e)
		{
			if (this.ZoomFactor != null && this.ZoomFactor.CanZoomUp)
			{
				this.ZoomFactor.ZoomUp();
			}
		}

		private void ChangeBackground()
		{
			if (this.Popup.IsOpen)
			{
				try
				{
					this.Background = this.FindResource("AccentBrushKey") as Brush;
				}
				catch (ResourceReferenceKeyNotFoundException ex)
				{
					Debug.WriteLine(ex);
				}
			}
			else if (this.IsMouseOver)
			{
				try
				{
					this.Background = this.FindResource("ActiveBackgroundBrushKey") as Brush;
				}
				catch (ResourceReferenceKeyNotFoundException ex)
				{
					Debug.WriteLine(ex);
				}
			}
			else
			{
				this.Background = Brushes.Transparent;
			}
		}
	}
}
