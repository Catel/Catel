namespace Catel.Tests.Data.Exceptions
{
    using Catel.Data;

    using NUnit.Framework;

    public class PropertyNotRegisteredExceptionFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void SetsValuesCorrectly()
            {
                var exception = new PropertyNotRegisteredException("PropertyName", typeof(string));
                Assert.That(exception.PropertyName, Is.EqualTo("PropertyName"));
                Assert.That(exception.ObjectType, Is.EqualTo(typeof(string)));
            }
        }
    }
}