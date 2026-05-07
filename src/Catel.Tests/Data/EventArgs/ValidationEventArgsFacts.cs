namespace Catel.Tests.Data;

using Catel.Data;
using Moq;
using NUnit.Framework;

public class ValidationEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_ValidationContext()
        {
            var validationContext = new Mock<IValidationContext>().Object;

            var eventArgs = new ValidationEventArgs(validationContext);

            Assert.That(eventArgs.ValidationContext, Is.EqualTo(validationContext));
        }
    }
}
