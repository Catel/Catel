namespace Catel.Tests;

using NUnit.Framework;

public class NotSupportedInPlatformExceptionFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_Reason_And_Platform()
        {
            var exception = new NotSupportedInPlatformException("{0} is missing", "Feature");

            Assert.That(exception.Reason, Is.EqualTo("Feature is missing"));
            Assert.That(exception.Platform, Is.EqualTo(Platforms.CurrentPlatform));
            Assert.That(exception.Message, Is.EqualTo("Feature is currently not yet supported for this platform"));
        }
    }
}
