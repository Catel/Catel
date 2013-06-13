// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualTargetPresentationSource.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Security.Permissions;
    using System.Windows;
    using System.Windows.Media;

	/// <summary>
	/// Support for multi-threaded host visuals.
	/// </summary>
	/// <remarks>
    /// The original code can be found here: http://blogs.msdn.com/b/dwayneneed/archive/2007/04/26/multithreaded-ui-hostvisual.aspx.
	/// </remarks>
	[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
    [UIPermission(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)]
	public class VisualTargetPresentationSource : PresentationSource
	{
		private readonly VisualTarget _visualTarget;

		/// <summary>
		/// Initializes a new instance of the <see cref="VisualTargetPresentationSource"/> class.
		/// </summary>
		/// <param name="hostVisual">The host visual.</param>
		public VisualTargetPresentationSource(HostVisual hostVisual)
		{
			_visualTarget = new VisualTarget(hostVisual);
		}

		/// <summary>
		/// When overridden in a derived class, gets or sets the root visual being presented in the source.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The root visual.
		/// </returns>
		public override Visual RootVisual
		{
			get { return _visualTarget.RootVisual; }
			set
			{
				Visual oldRoot = _visualTarget.RootVisual;

				// Set the root visual of the VisualTarget.  This visual will
				// now be used to visually compose the scene.
				_visualTarget.RootVisual = value;

				// Tell the PresentationSource that the root visual has
				// changed.  This kicks off a bunch of stuff like the
				// Loaded event.
				RootChanged(oldRoot, value);

				// Kickoff layout...
				UIElement rootElement = value as UIElement;
				if (rootElement != null)
				{
					rootElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
					rootElement.Arrange(new Rect(rootElement.DesiredSize));
				}
			}
		}

		/// <summary>
		/// When overridden in a derived class, returns a visual target for the given source.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="T:System.Windows.Media.CompositionTarget"/> that is target for rendering the visual.
		/// </returns>
		protected override CompositionTarget GetCompositionTargetCore()
		{
			return _visualTarget;
		}

		/// <summary>
		/// When overridden in a derived class, gets a value that declares whether the object is disposed.
		/// </summary>
		/// <value></value>
		/// <returns>true if the object is disposed; otherwise, false.
		/// </returns>
		public override bool IsDisposed
		{
			get
			{
				return false;
			}
		}
	}
}