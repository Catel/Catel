namespace Catel.Tests.MVVM.Commands;

using System.Windows.Input;
using Catel.MVVM;
using Moq;
using NUnit.Framework;

public class CommandCreatedEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_Command_And_Name()
        {
            var command = new Mock<ICommand>().Object;

            var eventArgs = new CommandCreatedEventArgs(command, "MyCommand");

            Assert.That(eventArgs.Command, Is.EqualTo(command));
            Assert.That(eventArgs.Name, Is.EqualTo("MyCommand"));
        }
    }
}
