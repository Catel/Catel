namespace Catel.Tests.MVVM.ViewModels.TestClasses;

using System;

public class ViewModelFactoryTestViewModelWithOnlyDefaultConstructor : ViewModelFactoryTestViewModel
{
    public ViewModelFactoryTestViewModelWithOnlyDefaultConstructor(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }
}
