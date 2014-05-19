namespace Catel.Test.IoC
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

    public class TestClass1 : ITestInterface, ITestInterface1, INotifyPropertyChanged
    {
        public TestClass1()
        {
            Name = "created via injection";
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class TestClass2 : ITestInterface, ITestInterface2
    {
        public TestClass2()
        {
            Name = "created via injection";
        }

        public string Name { get; set; }
    }
}
