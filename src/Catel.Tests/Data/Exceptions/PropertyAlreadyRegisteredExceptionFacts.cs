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
                Assert.AreEqual("PropertyName", exception.PropertyName);
                Assert.AreEqual(typeof(string), exception.PropertyType);
            }
        }
    }
}