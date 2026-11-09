namespace Catel.Tests.MVVM.ViewModels.TestClasses;

using System;
using Catel.MVVM;

public class ViewModelFactoryParentViewModelTestViewModel : ViewModelBase
{
    public ViewModelFactoryParentViewModelTestViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    public ViewModelFactoryParentViewModelTestViewModel(IViewModel parentViewModel, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        ParentViewModel = parentViewModel;
    }

    public IViewModel? ParentViewModel { get; }
}
