namespace Catel.Tests.Data;

using Catel.Data;
using NUnit.Framework;

public class ValidateModelAttributeFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_ValidatorType()
        {
            var attribute = new ValidateModelAttribute(typeof(string));

            Assert.That(attribute.ValidatorType, Is.EqualTo(typeof(string)));
        }
    }
}
