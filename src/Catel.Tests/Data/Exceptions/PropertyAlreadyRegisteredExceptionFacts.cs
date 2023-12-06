namespace Catel.Tests.Data.Exceptions
{
    using Catel.Data;

    using NUnit.Framework;

    public class PropertyAlreadyRegisteredExceptionFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void SetsValuesCorrectly()
            {
                var exception = new PropertyAlreadyRegisteredException("PropertyName", typeof(string));
                Assert.That(exception.PropertyName, Is.EqualTo("PropertyName"));
                Assert.That(exception.PropertyType, Is.EqualTo(typeof(string)));
            }
        }
    }
}