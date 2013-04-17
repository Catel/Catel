// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Page.generic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using MVVM;

    /// <summary>
    /// Generic implementation of the <see cref="Page"/> class that supports MVVM with Catel.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <remarks>
    /// Starting with Catel 3.0, the <see cref="Page{TViewModel}"/> now derives from <see cref="Page"/> and should
    /// only be used when there is an actual need to specify the class as a generic. Otherwise, it is always recommend to use the
    /// new <see cref="Page"/> which is not generic and can determine the view model by itself.
    /// </remarks>
    [ObsoleteEx(Replacement = "Catel.Windows.Controls.Page", TreatAsErrorFromVersion = "3.3", RemoveInVersion = "4.0")]
    public class Page<TViewModel> : Page
        where TViewModel : class, IViewModel
    {
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public new TViewModel ViewModel
        {
            get { return base.ViewModel as TViewModel; }
        }

        /// <summary>
        /// Gets the type of the view model. If this method returns <c>null</c>, the view model type will be retrieved by naming convention.
        /// </summary>
        /// <returns>The type of the view model or <c>null</c> in case it should be auto determined.</returns>
        /// <remarks></remarks>
        protected override Type GetViewModelType()
        {
            return typeof (TViewModel);
        }
    }
}