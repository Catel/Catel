namespace Catel.Tests.MVVM.ViewModels;

using Catel.MVVM;
using NUnit.Framework;

public class SavingEventArgsFacts
{
    [TestFixture]
    public class The_Cancel_Property
    {
        [Test]
        public void Allows_Setting_Cancel()
        {
            var eventArgs = new SavingEventArgs
            {
                Cancel = true
            };

            Assert.That(eventArgs.Cancel, Is.True);
        }
    }
}
