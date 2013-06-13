// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDataFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class PropertyDataFacts
    {
        [TestClass]
        public class TheDefaultValueProperty
        {
            [TestMethod]
            public void ReturnsDefaultValueForReferenceTypes()
            {
                var propertiesObject = new ObjectWithoutDefaultValues();

                Assert.AreNotEqual(null, propertiesObject.ReferenceType);
                Assert.AreEqual(null, propertiesObject.ReferenceTypeWithoutDefaultValue);
            }

            [TestMethod]
            public void ReturnsDefaultValueForValueTypes()
            {
                var propertiesObject = new ObjectWithoutDefaultValues();

                Assert.AreEqual(1, propertiesObject.ValueType);
                Assert.AreEqual(0, propertiesObject.ValueTypeWithoutDefaultValue);
            }
        }

        [TestClass]
        public class TheGetDefaultValueMethod
        {
            [TestMethod]
            public void ReturnsDefaultValueForReferenceType()
            {
                var property = ObjectWithoutDefaultValues.ReferenceTypeProperty;

                Assert.AreEqual(property.GetDefaultValue(), property.GetDefaultValue<object>());
            }

            [TestMethod]
            public void ReturnsDefaultValueForValueType()
            {
                var property = ObjectWithoutDefaultValues.ValueTypeProperty;

                Assert.AreEqual(property.GetDefaultValue(), property.GetDefaultValue<int>());
            }
        }
    }
}