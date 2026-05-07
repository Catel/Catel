namespace Catel.Tests.Data;

using Catel.Data;
using NUnit.Framework;

public class ExcludeFromValidationAttributeFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Can_Be_Instantiated()
        {
            var attribute = new ExcludeFromValidationAttribute();

            Assert.That(attribute, Is.Not.Null);
        }
    }
}
