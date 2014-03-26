// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationPageLogic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using Catel.MVVM.Views;
    using MVVM;

    /// <summary>
    /// MVVM Provider behavior implementation for a navigation page.
    /// </summary>
    public class PageLogic : NavigationLogicBase<IPage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageLogic"/> class.
        /// </summary>
        /// <param name="targetPage">The page this provider should take care of.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetPage"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> does not implement interface <see cref="IViewModel"/>.</exception>
        public PageLogic(IPage targetPage, Type viewModelType)
            : base(targetPage, viewModelType)
        {
        }

        /// <summary>
        /// Sets the data context of the target control.
        /// <para />
        /// This method is abstract because the real logic implementation knows how to set the data context (for example,
        /// by using an additional data context grid).
        /// </summary>
        /// <param name="newDataContext">The new data context.</param>
        protected override void SetDataContext(object newDataContext)
        {
            TargetView.DataContext = newDataContext;
        }
    }
}
