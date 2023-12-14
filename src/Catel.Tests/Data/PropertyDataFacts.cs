namespace Catel.Tests.Data
{
    using NUnit.Framework;

    public class PropertyDataFacts
    {
        [TestFixture]
        public class TheDefaultValueProperty
        {
            [TestCase]
            public void ReturnsDefaultValueForReferenceTypes()
            {
                var propertiesObject = new ObjectWithoutDefaultValues();

                Assert.That(propertiesObject.ReferenceType, Is.Not.EqualTo(null));
                Assert.That(propertiesObject.ReferenceTypeWithoutDefaultValue, Is.EqualTo(null));
            }

            [TestCase]
            public void ReturnsDefaultValueForValueTypes()
            {
                var propertiesObject = new ObjectWithoutDefaultValues();

                Assert.That(propertiesObject.ValueType, Is.EqualTo(1));
                Assert.That(propertiesObject.ValueTypeWithoutDefaultValue, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class TheGetDefaultValueMethod
        {
            [TestCase]
            public void ReturnsDefaultValueForReferenceType()
            {
                var property = ObjectWithoutDefaultValues.ReferenceTypeProperty;

                Assert.That(property.GetDefaultValue<object>(), Is.EqualTo(property.GetDefaultValue()));
            }

            [TestCase]
            public void ReturnsDefaultValueForValueType()
            {
                var property = ObjectWithoutDefaultValues.ValueTypeProperty;

                Assert.That(property.GetDefaultValue<int>(), Is.EqualTo(property.GetDefaultValue()));
            }
        }
    }
}