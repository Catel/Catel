namespace Catel.Tests.Data.Exceptions
{
    using Catel.Data;

    using NUnit.Framework;

    public class InvalidPropertyValueExceptionFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void SetsValuesCorrectly()
            {
                var exception = new InvalidPropertyValueException("PropertyName", typeof(int), typeof(string));
                Assert.That(exception.PropertyName, Is.EqualTo("PropertyName"));
                Assert.That(exception.ExpectedType, Is.EqualTo(typeof(int)));
                Assert.That(exception.ActualType, Is.EqualTo(typeof(string)));
            }
        }
    }
}