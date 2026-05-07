namespace Catel.Tests.MVVM.Providers;

using Catel.MVVM;
using Catel.MVVM.Providers;
using Moq;
using NUnit.Framework;

public class DetermineViewModelInstanceEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_DataContext_And_Allows_Setting_Properties()
        {
            var viewModel = new Mock<IViewModel>().Object;
            var eventArgs = new DetermineViewModelInstanceEventArgs("context")
            {
                DoNotCreateViewModel = true,
                ViewModel = viewModel
            };

            Assert.That(eventArgs.DataContext, Is.EqualTo("context"));
            Assert.That(eventArgs.DoNotCreateViewModel, Is.True);
            Assert.That(eventArgs.ViewModel, Is.EqualTo(viewModel));
        }
    }
}
