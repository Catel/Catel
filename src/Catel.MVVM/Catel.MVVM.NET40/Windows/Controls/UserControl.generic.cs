// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserControl.generic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using MVVM;

    /// <summary>
    /// <see cref="UserControl"/> that supports MVVM by using a <see cref="IViewModel"/> typed parameter.
    /// If the user control is not constructed with the right view model by the developer, it will try to create
    /// the view model itself. It does this by keeping an eye on the <c>DataContext</c> property. If
    /// the property changes, the control will check the type of the DataContext and try to create the view model by using
    /// the DataContext value as the constructor. If the view model can be constructed, the DataContext of the UserControl will
    /// be replaced by the view model.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <remarks>
    /// Starting with Catel 3.0, the <see cref="UserControl{TViewModel}"/> now derives from <see cref="UserControl"/> and should
    /// only be used when there is an actual need to specify the class as a generic. Otherwise, it is always recommend to use the
    /// new <see cref="UserControl"/> which is not generic and can determine the view model by itself.
    /// </remarks>
    [ObsoleteEx(Replacement = "Catel.Windows.Controls.UserControl", TreatAsErrorFromVersion = "3.3", RemoveInVersion = "4.0")]
    public class UserControl<TViewModel> : UserControl
        where TViewModel : class, IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.FrameworkElement"/> class.
        /// </summary>
        public UserControl()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControl{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public UserControl(TViewModel viewModel)
            : base(viewModel)
        {
        }

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