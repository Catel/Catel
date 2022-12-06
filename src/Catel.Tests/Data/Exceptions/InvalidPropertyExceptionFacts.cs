namespace Catel.Tests.Data.Exceptions
{
    using Catel.Data;

    using NUnit.Framework;

    public class InvalidPropertyExceptionFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void SetsValuesCorrectly()
            {
                var exception = new InvalidPropertyException("PropertyName");
                Assert.AreEqual("PropertyName", exception.PropertyName);
            }
        }
    }
}