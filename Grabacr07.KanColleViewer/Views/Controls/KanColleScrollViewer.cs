using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	[ContentProperty("WebBrowser")]
	[TemplatePart(Name = PART_ContentHost, Type = typeof(ScrollViewer))]
	public class KanColleScrollViewer : Control
	{
		// ReSharper disable InconsistentNaming
		private const string PART_ContentHost = "PART_ContentHost";
		// ReSharper restore InconsistentNaming

		static KanColleScrollViewer()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(KanColleScrollViewer), new FrameworkPropertyMetadata(typeof(KanColleScrollViewer)));
		}


		private ScrollViewer scrollViewer;

		#region WebBrowser 依存関係プロパティ

		public WebBrowser WebBrowser
		{
			get { return (WebBrowser)this.GetValue(WebBrowserProperty); }
			set { this.SetValue(WebBrowserProperty, value); }
		}
		public static readonly DependencyProperty WebBrowserProperty =
			DependencyProperty.Register("WebBrowser", typeof(WebBrowser), typeof(KanColleScrollViewer), new UIPropertyMetadata(null, WebBrowserPropertyChangedCallback));

		private static void WebBrowserPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (KanColleScrollViewer)d;
		}

		#endregion


		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.scrollViewer = this.GetTemplateChild(PART_ContentHost) as ScrollViewer;
			if (this.scrollViewer != null)
			{
				
			}
		}
	}
}
