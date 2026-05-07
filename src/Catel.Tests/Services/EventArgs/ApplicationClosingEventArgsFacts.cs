namespace Catel.Tests.Services.EventArgs;

using Catel.Services;
using NUnit.Framework;

public class ApplicationClosingEventArgsFacts
{
    [TestFixture]
    public class The_Cancel_Property
    {
        [Test]
        public void Defaults_Cancel_To_False_And_Allows_Updates()
        {
            var eventArgs = new ApplicationClosingEventArgs();

            Assert.That(eventArgs.Cancel, Is.False);

            eventArgs.Cancel = true;

            Assert.That(eventArgs.Cancel, Is.True);
        }
    }
}
