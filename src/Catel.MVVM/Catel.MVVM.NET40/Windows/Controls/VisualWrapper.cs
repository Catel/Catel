// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualWrapper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using System.Windows;


#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Markup;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows.Markup;
    using System.Windows.Media;
#endif

    /// <summary>
    /// This visual wrapper is used by VisualTargetPresentationSource
    /// </summary>
#if NETFX_CORE
	[ContentProperty(Name = "Child")]
#else
    [ContentProperty("Child")]
#endif
    public class VisualWrapper : FrameworkElement
    {
        private Visual _child;

        /// <summary>w
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        public Visual Child
        {
            get
            {
                return _child;
            }

            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                }

                _child = value;

                if (_child != null)
                {
                    AddVisualChild(_child);
                }
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (_child != null && index == 0)
            {
                return _child;
            }

            throw new ArgumentOutOfRangeException("index");
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of visual child elements for this element.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get { return _child != null ? 1 : 0; }
        }
    }
}