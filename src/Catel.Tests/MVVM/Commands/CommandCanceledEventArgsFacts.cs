namespace Catel.Tests.MVVM.Commands;

using Catel.MVVM;
using NUnit.Framework;

public class CommandCanceledEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_CommandParameter_And_Default_Cancel()
        {
            var eventArgs = new CommandCanceledEventArgs("value");

            Assert.That(eventArgs.CommandParameter, Is.EqualTo("value"));
            Assert.That(eventArgs.Cancel, Is.False);
        }
    }
}
