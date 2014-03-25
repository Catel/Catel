// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWrapper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;

#if NETFX_CORE
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// View model wrapper class.
    /// </summary>
    public class ViewModelWrapper : IViewModelWrapper
    {
#if !XAMARIN
        private readonly Grid _grid;
#else

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelWrapper"/> class.
        /// </summary>
        /// <param name="viewModelWrapper">The view model wrapper object, such as a grid.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelWrapper"/> is <c>null</c>.</exception>
        public ViewModelWrapper(object viewModelWrapper)
        {
            Argument.IsNotNull("viewModelWrapper", viewModelWrapper);

#if !XAMARIN
            _grid = (Grid) viewModelWrapper;
#else
            throw new MustBeImplementedException();
#endif
        }

        /// <summary>
        /// Updates the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void UpdateViewModel(IViewModel viewModel)
        {
#if !XAMARIN
            _grid.DataContext = viewModel;
#else
            throw new MustBeImplementedException();
#endif
        }
    }
}