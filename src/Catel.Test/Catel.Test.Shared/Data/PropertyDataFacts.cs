﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDataFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
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

                Assert.AreNotEqual(null, propertiesObject.ReferenceType);
                Assert.AreEqual(null, propertiesObject.ReferenceTypeWithoutDefaultValue);
            }

            [TestCase]
            public void ReturnsDefaultValueForValueTypes()
            {
                var propertiesObject = new ObjectWithoutDefaultValues();

                Assert.AreEqual(1, propertiesObject.ValueType);
                Assert.AreEqual(0, propertiesObject.ValueTypeWithoutDefaultValue);
            }
        }

        [TestFixture]
        public class TheGetDefaultValueMethod
        {
            [TestCase]
            public void ReturnsDefaultValueForReferenceType()
            {
                var property = ObjectWithoutDefaultValues.ReferenceTypeProperty;

                Assert.AreEqual(property.GetDefaultValue(), property.GetDefaultValue<object>());
            }

            [TestCase]
            public void ReturnsDefaultValueForValueType()
            {
                var property = ObjectWithoutDefaultValues.ValueTypeProperty;

                Assert.AreEqual(property.GetDefaultValue(), property.GetDefaultValue<int>());
            }
        }
    }
}