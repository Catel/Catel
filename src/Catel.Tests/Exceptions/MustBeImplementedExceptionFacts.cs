namespace Catel.Tests;

using NUnit.Framework;

public class MustBeImplementedExceptionFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Contains_Default_Message()
        {
            var exception = new MustBeImplementedException();

            Assert.That(exception.Message, Does.Contain("limited resources"));
        }
    }
}
