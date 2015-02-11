﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDataTypeInfoFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using System;
    using System.Linq;
    using Catel.Data;

    using NUnit.Framework;

    [TestFixture]
    public class CatelTypeInfoFacts
    {
        public class CatelTypeInfoTestModel : ModelBase
        {
            public string NormalProperty { get; set; }

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            public string CatelProperty
            {
                get { return GetValue<string>(CatelPropertyProperty); }
                set { SetValue(CatelPropertyProperty, value); }
            }

            /// <summary>
            /// Register the CatelProperty property so it is known in the class.
            /// </summary>
            public static readonly PropertyData CatelPropertyProperty = RegisterProperty("CatelProperty", typeof(string), null);
        }

        [TestCase]
        public void CorrectlyRegistersCatelProperties()
        {
            var catelTypeInfo = new CatelTypeInfo(typeof (CatelTypeInfoTestModel));

            var properties = catelTypeInfo.GetCatelProperties();
            Assert.AreNotEqual(0, properties.Count);
            Assert.IsTrue(properties.Keys.Contains("CatelProperty"));
        }

        [TestCase]
        public void CorrectlyRegistersNonCatelProperties()
        {
            var catelTypeInfo = new CatelTypeInfo(typeof(CatelTypeInfoTestModel));

            var properties = catelTypeInfo.GetNonCatelProperties();
            Assert.AreNotEqual(0, properties.Count);
            Assert.IsTrue(properties.Keys.Contains("NormalProperty"));
        }
    }
}