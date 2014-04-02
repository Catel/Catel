// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIView.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MonoTouch.UIKit
{
    using System.Drawing;
    using Catel.MVVM.Views;
    using global::MonoTouch.Foundation;
    using global::MonoTouch.UIKit;

    /// <summary>
    /// UIView implementation that automatically takes care of view models.
    /// </summary>
    [Register("UIView")]
    public class UIView : global::MonoTouch.UIKit.UIView, IUserControl
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UIView"/> class.
        /// </summary>
        public UIView()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIView"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        public UIView(RectangleF bounds)
            : base(bounds)
        {
            Initialize();
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            
        }
        #endregion
    }
}