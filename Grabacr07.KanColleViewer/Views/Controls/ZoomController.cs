using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Grabacr07.KanColleViewer.Models;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	[TemplatePart(Name = PART_ZoomUpButton, Type = typeof(Button))]
	[TemplatePart(Name = PART_ZoomDownButton, Type = typeof(Button))]
	[TemplatePart(Name = PART_CurrentValue, Type = typeof(TextBlock))]
	[TemplatePart(Name = PART_ContextMenuButton, Type = typeof(Button))]
	public class ZoomController : Control
	{
		private const string PART_ZoomUpButton = "PART_ZoomUpButton";
		private const string PART_ZoomDownButton = "PART_ZoomDownButton";
		private const string PART_CurrentValue = "PART_CurrentValue";
		private const string PART_ContextMenuButton = "PART_ContextMenuButton";

		static ZoomController()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomController), new FrameworkPropertyMetadata(typeof(ZoomController)));
		}

		#region ZoomFactor 依存関係プロパティ

		public IZoomFactor ZoomFactor
		{
			get { return (IZoomFactor)this.GetValue(ZoomFactorProperty); }
			set { this.SetValue(ZoomFactorProperty, value); }
		}
		public static readonly DependencyProperty ZoomFactorProperty =
			DependencyProperty.Register("ZoomFactor", typeof(IZoomFactor), typeof(ZoomController), new UIPropertyMetadata(null));

		#endregion


		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var zoomUpButton = this.GetTemplateChild(PART_ZoomUpButton) as Button;
			if (zoomUpButton != null)
			{
				zoomUpButton.Click += (sender, args) =>
				{
					if (this.ZoomFactor != null) this.ZoomFactor.ZoomUp();
				};

				var binding = new Binding("ZoomFactor.CanZoomUp")
				{
					Source = this,
					Mode = BindingMode.OneWay,
				};
				zoomUpButton.SetBinding(IsEnabledProperty, binding);
			}

			var zoomDownButton = this.GetTemplateChild(PART_ZoomDownButton) as Button;
			if (zoomDownButton != null)
			{
				zoomDownButton.Click += (sender, args) =>
				{
					if (this.ZoomFactor != null) this.ZoomFactor.ZoomDown();
				};

				var binding = new Binding("ZoomFactor.CanZoomDown")
				{
					Source = this,
					Mode = BindingMode.OneWay,
				};
				zoomDownButton.SetBinding(IsEnabledProperty, binding);
			}

			var textBlock = this.GetTemplateChild(PART_CurrentValue) as TextBlock;
			if (textBlock != null)
			{
				var binding = new Binding("ZoomFactor.CurrentParcentage")
				{
					Source = this,
					Mode = BindingMode.OneWay,
				};
				textBlock.SetBinding(TextBlock.TextProperty, binding);
			}

			var showDialogButton = this.GetTemplateChild(PART_ContextMenuButton) as Button;
			if (showDialogButton != null)
			{
				showDialogButton.Click += (sender, args) =>
				{
				};
			}
		}


	}
}
