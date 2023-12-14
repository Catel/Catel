namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using System;
    using Catel.MVVM;

    public class ViewModelFactoryTestViewModel : ViewModelBase
    {
        public ViewModelFactoryTestViewModel()
        {
            EmptyConstructorCalled = true;
        }

        public ViewModelFactoryTestViewModel(int integer)
        {
            Integer = integer;
        }

        public ViewModelFactoryTestViewModel(bool boolean)
        {
            Boolean = boolean;
        }

        public ViewModelFactoryTestViewModel(string stringvalue)
        {
            throw new NotSupportedException(stringvalue);
        }

        public ViewModelFactoryTestViewModel(int integer, IDummyDependency dependency)
        {
            Integer = integer;
            Dependency = dependency;
        }

        public ViewModelFactoryTestViewModel(IDummyDependency dependency)
        {
            Dependency = dependency;
        }

        public bool Boolean { get; set; }

        public int Integer { get; set; }

        public bool EmptyConstructorCalled { get; set; }

        public IDummyDependency Dependency { get; set; }
    }
}