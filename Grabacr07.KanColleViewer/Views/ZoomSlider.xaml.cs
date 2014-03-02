using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Grabacr07.KanColleViewer.Views
{
	public partial class ZoomSlider
	{
		private readonly double?[] zoomTable =
		{
			0.25, 0.50, 0.75,
			1.00, 1.25, 1.50, 1.75,
			2.00, 2.50,
			3.00,
			4.00,
		};

		private static double ZoomFactor
		{
			get { return App.ViewModelRoot.Settings.BrowserZoomFactor; }
			set { App.ViewModelRoot.Settings.BrowserZoomFactor = value; }
		}


		public ZoomSlider()
		{
			this.InitializeComponent();

			var binding = new Binding("BrowserZoomFactor") { Source = App.ViewModelRoot.Settings };
			this.Slider.SetBinding(RangeBase.ValueProperty, binding);
		}


		private void ZoomUp(object sender, RoutedEventArgs e)
		{
			var current = ZoomFactor;
			ZoomFactor = zoomTable.FirstOrDefault(x => current < x) ?? zoomTable.Last() ?? 1.0;
		}

		private void ZoomDown(object sender, RoutedEventArgs e)
		{
			var current = ZoomFactor;
			ZoomFactor = zoomTable.LastOrDefault(x => x < current) ?? zoomTable.First() ?? 1.0;
		}
	}
}
