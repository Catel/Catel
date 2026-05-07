namespace Catel.Tests.MVVM.Exceptions;

using Catel.MVVM;
using NUnit.Framework;

public class InvalidViewModelExceptionFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_Message()
        {
            var exception = new InvalidViewModelException("invalid");

            Assert.That(exception.Message, Is.EqualTo("invalid"));
        }
    }
}
