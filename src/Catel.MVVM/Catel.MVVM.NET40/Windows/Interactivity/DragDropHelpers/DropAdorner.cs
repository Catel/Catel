// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropAdorner.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity.DragDropHelpers
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    /// <summary>
	/// Handles the visual indication of the drop point
	/// </summary>
	internal class DropAdorner : Adorner, IDisposable
	{
        public bool IsTopHalf { get; set; }

		private readonly AdornerLayer _myAdornerLayer;
		private readonly Pen _myPen;
		private readonly bool _myDrawHorizontal;

		/// <summary>
		/// Initializes a new instance of the <see cref="DropAdorner"/> class.
		/// </summary>
		/// <param name="isTopHalf">if set to <c>true</c> we are adorning the top half of the item.</param>
		/// <param name="drawHorizontal">if set to <c>true</c> the item being adorned has a horizontal orientation.</param>
		/// <param name="adornedElement">The adorned element.</param>
		/// <param name="adornerLayer">The adorner layer.</param>
		public DropAdorner(bool isTopHalf, bool drawHorizontal, UIElement adornedElement, AdornerLayer adornerLayer)
			: base(adornedElement)
		{
            Argument.IsNotNull("adornedElement", adornedElement);
            Argument.IsNotNull("adornerLayer", adornerLayer);

			IsTopHalf = isTopHalf;
			_myAdornerLayer = adornerLayer;
			_myDrawHorizontal = drawHorizontal;
			
			IsHitTestVisible = false;

			_myAdornerLayer.Add(this);

		    var animation = new DoubleAnimation(0.5, 1, new Duration(TimeSpan.FromSeconds(0.5))) {AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever};

		    _myPen = new Pen(new SolidColorBrush(Colors.Red), 3.0);
			_myPen.Brush.BeginAnimation(Brush.OpacityProperty, animation);
		}

		/// <summary>
		/// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
		/// </summary>
		/// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			if (drawingContext == null) throw new ArgumentNullException("drawingContext");

		    var points = _myDrawHorizontal ? DetermineHorizontalLinePoints() : DetermineVerticalLinePoints();

			drawingContext.DrawLine(_myPen, points.First(), points.Last());
		}

		private Point[] DetermineHorizontalLinePoints()
		{
			var width = AdornedElement.RenderSize.Width;
			var height = AdornedElement.RenderSize.Height;

			Point startPoint;
			Point endPoint;
			if (IsTopHalf)
			{
				startPoint = new Point(0, 0);
				endPoint = new Point(0, height);
			}
			else
			{
				startPoint = new Point(width, 0);
				endPoint = new Point(width, height);
			}

			return new[] { startPoint, endPoint };
		}

		private Point[] DetermineVerticalLinePoints()
		{
			var width = AdornedElement.RenderSize.Width;
			var height = AdornedElement.RenderSize.Height;

			Point startPoint;
			Point endPoint;
			if (IsTopHalf)
			{
				startPoint = new Point(0, 0);
				endPoint = new Point(width, 0);
			}
			else
			{
				startPoint = new Point(0, height);
				endPoint = new Point(width, height);
			}
			
			return new[] { startPoint, endPoint };
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//Free managed resources
				_myAdornerLayer.Remove(this);
			}
		}
		
		#endregion
	}
}