namespace Catel.Tests.Reflection;

using Catel.Reflection;
using NUnit.Framework;

public class ReflectionExceptionsFacts
{
    [TestFixture]
    public class The_CannotGetPropertyValueException_Constructor
    {
        [Test]
        public void Sets_PropertyName_And_Message()
        {
            var exception = new CannotGetPropertyValueException("MyProperty");

            Assert.That(exception.PropertyName, Is.EqualTo("MyProperty"));
            Assert.That(exception.Message, Does.Contain("MyProperty"));
        }
    }

    [TestFixture]
    public class The_CannotSetPropertyValueException_Constructor
    {
        [Test]
        public void Sets_PropertyName_And_Message()
        {
            var exception = new CannotSetPropertyValueException("MyProperty");

            Assert.That(exception.PropertyName, Is.EqualTo("MyProperty"));
            Assert.That(exception.Message, Does.Contain("MyProperty"));
        }
    }

    [TestFixture]
    public class The_PropertyNotFoundException_Constructor
    {
        [Test]
        public void Sets_PropertyName_And_Message()
        {
            var exception = new PropertyNotFoundException("MissingProperty");

            Assert.That(exception.PropertyName, Is.EqualTo("MissingProperty"));
            Assert.That(exception.Message, Does.Contain("MissingProperty"));
        }
    }
}
