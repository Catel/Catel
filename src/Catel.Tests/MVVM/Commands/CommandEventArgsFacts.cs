namespace Catel.Tests.MVVM.Commands;

using Catel.MVVM;
using NUnit.Framework;

public class CommandEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_CommandParameter()
        {
            var eventArgs = new CommandEventArgs("value");

            Assert.That(eventArgs.CommandParameter, Is.EqualTo("value"));
        }
    }
}
