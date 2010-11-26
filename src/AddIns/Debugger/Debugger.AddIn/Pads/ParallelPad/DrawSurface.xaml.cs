﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Debugger.AddIn.Pads.ParallelPad
{
	public partial class DrawSurface : UserControl
	{
		Point dragStartedPoint;
		
		TransformGroup group = new TransformGroup();
		ScaleTransform zoom = new ScaleTransform();
		TranslateTransform translate = new TranslateTransform();
		
		public DrawSurface()
		{
			InitializeComponent();
		
			group.Children.Add(zoom);
			group.Children.Add(translate);			
			drawingSurface.RenderTransform = group;
			
			this.MouseLeftButtonDown += DrawSurface_PreviewMouseLeftButtonDown;
			this.MouseLeftButtonUp += DrawSurface_MouseLeftButtonUp;
		}
		
		public void SetGraph(ParallelStacksGraph graph)
		{
			this.ParallelStacksLayout.Graph = graph;
		}

		#region Pan
		
		void DrawSurface_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource is Slider || e.OriginalSource is Button)
				return;
			
			dragStartedPoint = e.GetPosition(drawingSurface);
			drawingSurface.CaptureMouse();
			this.PreviewMouseMove += DrawSurface_PreviewMouseMove;
			e.Handled = true;
		}

		void DrawSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			drawingSurface.ReleaseMouseCapture();
			if (e.OriginalSource is Slider || e.OriginalSource is Button)
				return;
			
			this.PreviewMouseMove -= DrawSurface_PreviewMouseMove;
			Cursor = Cursors.Arrow;
		}

		void DrawSurface_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (!drawingSurface.IsMouseCaptured) return;
			
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (e.OriginalSource is Slider || e.OriginalSource is Button)
					return;
				
				Cursor = Cursors.SizeAll;
				var point = e.GetPosition(drawingSurface);
				Vector v = dragStartedPoint - point;
				translate.X += v.X / 200;
				translate.Y += v.Y / 200;				
				e.Handled = true;
			}
		}
		
		#endregion
		
		#region Zoom
		
		void SliderControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (e.OldValue == 0)
				return;
			
			double value = (e.NewValue / 5d) * 100;
			
			this.PercentText.Text = string.Format("{0}%", value);
			
			// zoom canvas
			zoom.ScaleX = e.NewValue / 5d;
			zoom.ScaleY = e.NewValue / 5d;
			zoom.CenterX = drawingSurface.ActualWidth / 2d;
			zoom.CenterY = drawingSurface.ActualHeight / 2d;
		}
		
		void Reset_Click(object sender, RoutedEventArgs e)
		{
			this.SliderControl.Value = 5;
			
			translate.X = 0;
			translate.Y = 0;
		}
		
		#endregion
	}
}