namespace Catel.Tests.MVVM.ViewModels.TestClasses;

using System;
using Catel.MVVM;

public class ViewModelFactoryTestViewModel : ViewModelBase
{
    public ViewModelFactoryTestViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        EmptyConstructorCalled = true;
    }

    public ViewModelFactoryTestViewModel(int integer, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Integer = integer;
    }

    public ViewModelFactoryTestViewModel(bool boolean, IServiceProvider serviceProvider) 
        : base(serviceProvider)
    {
        Boolean = boolean;
    }

    public ViewModelFactoryTestViewModel(string stringValue, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        throw new NotSupportedException(stringValue);
    }

    public ViewModelFactoryTestViewModel(int integer, IServiceProvider serviceProvider, IDummyDependency dependency)
        : base(serviceProvider)
    {
        Integer = integer;
        Dependency = dependency;
    }

    public ViewModelFactoryTestViewModel(IServiceProvider serviceProvider, IDummyDependency dependency)
        : base(serviceProvider)
    {
        Dependency = dependency;
    }

    public bool Boolean { get; set; }

    public int Integer { get; set; }

    public bool EmptyConstructorCalled { get; set; }

    public IDummyDependency Dependency { get; set; }
}
