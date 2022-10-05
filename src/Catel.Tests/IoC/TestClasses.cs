namespace Catel.Tests.IoC
{
    using System.ComponentModel;

    public interface ITestInterface
    {
        string Name { get; set; }
    }

    public interface ITestInterface1
    {

    }

    public interface ITestInterface2
    {

    }

    public interface ITestInterface3
    {
        ITestInterface1 TestInterface1 { get; }
    }

    public class TestClass1 : ITestInterface, ITestInterface1, INotifyPropertyChanged
    {
        public TestClass1()
        {
            Name = "created via injection";

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
        }

        public string Name { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class TestClass2 : ITestInterface, ITestInterface2
    {
        public TestClass2()
        {
            Name = "created via injection";
        }

        public string Name { get; set; }
    }

    public class TestClass3 : ITestInterface, ITestInterface3
    {
        public TestClass3(ITestInterface1 testInterface1)
        {
            TestInterface1 = testInterface1;

            Name = "created via injection";
        }

        public string Name { get; set; }

        public ITestInterface1 TestInterface1 { get; }
    }
}
