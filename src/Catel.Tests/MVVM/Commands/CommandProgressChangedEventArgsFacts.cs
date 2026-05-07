namespace Catel.Tests.MVVM.Commands;

using Catel.MVVM;
using NUnit.Framework;

public class CommandProgressChangedEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_Progress()
        {
            var eventArgs = new CommandProgressChangedEventArgs<int>(42);

            Assert.That(eventArgs.Progress, Is.EqualTo(42));
        }
    }
}
