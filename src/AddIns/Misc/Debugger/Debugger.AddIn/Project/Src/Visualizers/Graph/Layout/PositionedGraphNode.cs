﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Graph.Drawing;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ObjectNode with added position information.
	/// </summary>
	public class PositionedGraphNode
	{
		/// <summary>
		/// Creates new PositionedNode.
		/// </summary>
		/// <param name="objectNode">Underlying ObjectNode.</param>
		public PositionedGraphNode(ObjectGraphNode objectNode)
		{
			this.objectNode = objectNode;
		}
		
		private ObjectGraphNode objectNode;
		/// <summary>
		/// Underlying ObjectNode.
		/// </summary>
		public ObjectGraphNode ObjectNode
		{
			get { return objectNode; }
		}
		
		public event EventHandler<PositionedPropertyEventArgs> PropertyExpanded;
		public event EventHandler<PositionedPropertyEventArgs> PropertyCollapsed;
		
		// TODO posNodeForObjectGraphNode will be a service, that will return existing posNodes or create empty new
		public void InitContentFromObjectNode()
		{
			this.Content = new NestedNodeViewModel(this);
			this.Content.InitFrom(this.ObjectNode.Content);
		}
		
		public void InitView()
		{
			this.nodeVisualControl = new NodeControl();
			
			this.nodeVisualControl.PropertyExpanded += new EventHandler<PositionedPropertyEventArgs>(NodeVisualControl_Expanded);
			this.nodeVisualControl.PropertyCollapsed += new EventHandler<PositionedPropertyEventArgs>(NodeVisualControl_Collapsed);
		}
		
		public void FillView()
		{
			//this.nodeVisualControl.Root = this.Content;
			foreach (var property in this.Properties)
			{
				property.Evaluate();
				this.nodeVisualControl.AddProperty(property);
			}
		}
		
		/// <summary>
		/// Tree-of-properties content of this node.
		/// </summary>
		public NestedNodeViewModel Content 
		{ 
			get; set;
		}
		
		// TODO do proper tree iteration
		public IEnumerable<PositionedNodeProperty> Properties
		{
			get
			{
				foreach (var child in this.Content.Children)
				{
					var property = ((PropertyNodeViewModel)child).Property;
					yield return property;
				}
			}
		}
		
		private NodeControl nodeVisualControl;
		/// <summary>
		/// Visual control to be shown for this node.
		/// </summary>
		public NodeControl NodeVisualControl
		{
			get
			{
				return this.nodeVisualControl;
			}
		}
		
		public virtual IEnumerable<PositionedEdge> Edges
		{
			get
			{
				foreach	(PositionedNodeProperty property in this.Properties)
				{
					if (property.Edge != null)
						yield return property.Edge;
				}
			}
		}
		
		/*public PositionedNodeProperty AddProperty(ObjectGraphProperty objectProperty, bool isExpanded)
		{
			var newProperty = new PositionedNodeProperty(objectProperty, this);
			newProperty.IsExpanded = isExpanded;
			this.Properties.Add(newProperty);
			this.nodeVisualControl.AddProperty(newProperty);
			
			return newProperty;
		}*/

		public void Measure()
		{
			this.nodeVisualControl.Measure(new Size(500, 500));
		}
		
		public double Left { get; set; }
		public double Top { get; set; }
		public double Width
		{
			get { return NodeVisualControl.DesiredSize.Width; }
		}
		public double Height
		{
			get { return NodeVisualControl.DesiredSize.Height; }
		}
		
		public Point LeftTop
		{
			get { return new Point(Left, Top); }
		}
		
		public Point Center
		{
			get { return new Point(Left + Width / 2, Top + Height / 2); }
		}
		
		public Rect Rect { get {  return new Rect(Left, Top, Width, Height); } }
		
		#region event helpers
		private void NodeVisualControl_Expanded(object sender, PositionedPropertyEventArgs e)
		{
			// propagage event
			OnPropertyExpanded(this, e);
		}
		
		private void NodeVisualControl_Collapsed(object sender, PositionedPropertyEventArgs e)
		{
			// propagate event
			OnPropertyCollapsed(this, e);
		}
		
		protected virtual void OnPropertyExpanded(object sender, PositionedPropertyEventArgs propertyArgs)
		{
			if (this.PropertyExpanded != null)
			{
				this.PropertyExpanded(sender, propertyArgs);
			}
		}

		protected virtual void OnPropertyCollapsed(object sender, PositionedPropertyEventArgs propertyArgs)
		{
			if (this.PropertyCollapsed != null)
			{
				this.PropertyCollapsed(sender, propertyArgs);
			}
		}
		#endregion
	}
}
