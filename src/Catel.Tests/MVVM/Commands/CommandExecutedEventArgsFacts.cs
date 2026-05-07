namespace Catel.Tests.MVVM.Commands;

using System;
using Catel.MVVM;
using Moq;
using NUnit.Framework;

public class CommandExecutedEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_All_Values()
        {
            var command = new Mock<ICatelCommand>().Object;

            var eventArgs = new CommandExecutedEventArgs(command, "parameter", "MyCommand");

            Assert.That(eventArgs.Command, Is.EqualTo(command));
            Assert.That(eventArgs.CommandParameter, Is.EqualTo("parameter"));
            Assert.That(eventArgs.CommandPropertyName, Is.EqualTo("MyCommand"));
        }

        [Test]
        public void Throws_For_Null_Command()
        {
            Assert.Throws<ArgumentNullException>(() => new CommandExecutedEventArgs(null!));
        }
    }
}
