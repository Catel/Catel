namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    public interface IDummyDependency
    {
        string Value { get; set; }
    }

    public class DummyDependency : IDummyDependency
    {
        public string Value { get; set; }
    }
}