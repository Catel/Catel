namespace Catel.Tests.MVVM.Providers;

using Catel.MVVM.Providers;
using NUnit.Framework;

public class DetermineViewModelTypeEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_DataContext_And_ViewModelType()
        {
            var eventArgs = new DetermineViewModelTypeEventArgs("context")
            {
                ViewModelType = typeof(object)
            };

            Assert.That(eventArgs.DataContext, Is.EqualTo("context"));
            Assert.That(eventArgs.ViewModelType, Is.EqualTo(typeof(object)));
        }
    }
}
