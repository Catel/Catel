namespace Catel.Tests.MVVM.ViewModels.TestClasses;

using System;
using Catel.MVVM;

public class ViewModelFactoryModelInjectionOnlyTestViewModel : ViewModelBase
{
    public ViewModelFactoryModelInjectionOnlyTestViewModel(string model, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Model = model;
    }

    public string Model { get; }
}
