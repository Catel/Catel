namespace Catel.Tests.MVVM.ViewModels;

using Catel.MVVM;
using NUnit.Framework;

public class CancelingEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Defaults_Cancel_To_False()
        {
            var eventArgs = new CancelingEventArgs();

            Assert.That(eventArgs.Cancel, Is.False);
        }
    }
}
