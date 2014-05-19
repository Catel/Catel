// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWrapper.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.MVVM.Views
{
#if NETFX_CORE
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows.Controls;
#endif

    public partial class ViewModelWrapper
    {
        private Grid _grid;

        partial void CreateWrapper(object viewModelWrapper)
        {
            _grid = (Grid)viewModelWrapper;
        }

        partial void SetViewModel(IViewModel viewModel)
        {
            _grid.DataContext = viewModel;
        }
    }
}

#endif