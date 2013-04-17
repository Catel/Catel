// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneApplicationPage.generic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Phone.Controls
{
    using System;
    using MVVM;

    /// <summary>
    /// <see cref="PhoneApplicationPage"/> that supports MVVM by using a <see cref="IViewModel"/> typed parameter.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <remarks>
    /// Starting with Catel 3.0, the <see cref="PhoneApplicationPage{TViewModel}"/> now derives from <see cref="PhoneApplicationPage"/> and should
    /// only be used when there is an actual need to specify the class as a generic. Otherwise, it is always recommend to use the
    /// new <see cref="PhoneApplicationPage"/> which is not generic and can determine the view model by itself.
    /// </remarks>
    [ObsoleteEx(Replacement = "Catel.Phone.Controls.PhoneApplicationPage", TreatAsErrorFromVersion = "3.3", RemoveInVersion = "4.0")]
    [CLSCompliant(false)]
    public class PhoneApplicationPage<TViewModel> : PhoneApplicationPage
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