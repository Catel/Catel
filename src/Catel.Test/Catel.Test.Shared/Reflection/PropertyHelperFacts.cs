﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System;
    using Catel.Reflection;
    using NUnit.Framework;

    public partial class PropertyHelperFacts
    {
        #region Test classes
        public class MyPropertyHelperClass
        {
            #region Fields
            private readonly int _privateWriteField;
            private int _privateReadField;
            #endregion

            #region Constructors
            public MyPropertyHelperClass()
            {
                PublicProperty = 1;
                _privateReadField = 2;
                _privateWriteField = 4;
            }
            #endregion

            #region Properties
            public int PublicProperty { get; set; }

            public int PrivateReadProperty
            {
                set { _privateReadField = value; }
            }

            public int PrivateWriteProperty
            {
                get { return _privateWriteField; }
            }

            public string StringValue { get; set; }
            #endregion

            #region Methods
            public int GetPrivateReadPropertyValue()
            {
                return _privateReadField;
            }
            #endregion
        }
        #endregion

        [TestFixture]
        public class TheIsPropertyAvailableMethod
        {
            [TestCase]
            public void IsPropertyAvailable_NullInput()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.IsPropertyAvailable(null, "PublicProperty"));
            }

            [TestCase]
            public void IsPropertyAvailable_NotExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.AreEqual(false, PropertyHelper.IsPropertyAvailable(myPropertyHelperClass, "NotExistingProperty"));
            }

            [TestCase]
            public void IsPropertyAvailable_ExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.AreEqual(true, PropertyHelper.IsPropertyAvailable(myPropertyHelperClass, "PublicProperty"));
            }
        }

        [TestFixture]
        public class TheTryGetPropertyValueMethod
        {
            [TestCase]
            public void TryGetPropertyValue_ObjectNull()
            {
                object value;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.TryGetPropertyValue(null, "property", out value));
            }

            [TestCase]
            public void TryGetPropertyValue_PropertyNameNull()
            {
                object value;
                var obj = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => PropertyHelper.TryGetPropertyValue(obj, null, out value));
            }

            [TestCase]
            public void TryGetPropertyValue_NonExistingProperty()
            {
                object value;
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TryGetPropertyValue(obj, "Non-existing property", out value);

                Assert.IsFalse(result);
                Assert.AreEqual(null, value);
            }

            [TestCase]
            public void TryGetPropertyValue_ExistingProperty()
            {
                object value;
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TryGetPropertyValue(obj, "PublicProperty", out value);

                Assert.IsTrue(result);
                Assert.AreEqual(1, value);
            }
        }

        [TestFixture]
        public class TheGetPropertyValueMethod
        {
            [TestCase]
            public void GetPropertyValue_NullInput()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.GetPropertyValue(null, "PublicProperty"));
            }

            [TestCase]
            public void GetPropertyValue_NotExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<PropertyNotFoundException>(() => PropertyHelper.GetPropertyValue(myPropertyHelperClass, "NotExistingProperty"));
            }

            [TestCase]
            public void GetPropertyValue_PrivateReadProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<CannotGetPropertyValueException>(() => PropertyHelper.GetPropertyValue(myPropertyHelperClass, "PrivateReadProperty"));
            }

            [TestCase]
            public void GetPropertyValue_PrivateWriteProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.AreEqual(4, PropertyHelper.GetPropertyValue<int>(myPropertyHelperClass, "PrivateWriteProperty"));
            }

            [TestCase]
            public void GetPropertyValue_PublicProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                myPropertyHelperClass.PublicProperty = 42;
                Assert.AreEqual(42, PropertyHelper.GetPropertyValue<int>(myPropertyHelperClass, "PublicProperty"));
            }

            [TestCase]
            public void GetPropertyValue_StringValue()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                myPropertyHelperClass.StringValue = "FourtyTwo";
                Assert.AreEqual("FourtyTwo", PropertyHelper.GetPropertyValue<string>(myPropertyHelperClass, "StringValue"));
            }
        }

        [TestFixture]
        public class TrySetPropertyValueMethod
        {
            [TestCase]
            public void TrySetPropertyValue_ObjectNull()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.TrySetPropertyValue(null, "property", null));
            }

            [TestCase]
            public void TrySetPropertyValue_PropertyNameNull()
            {
                var obj = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => PropertyHelper.TrySetPropertyValue(obj, null, null));
            }

            [TestCase]
            public void TrySetPropertyValue_NonExistingProperty()
            {
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TrySetPropertyValue(obj, "Non-existing property", null);

                Assert.IsFalse(result);
            }

            [TestCase]
            public void TrySetPropertyValue_ExistingProperty()
            {
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TrySetPropertyValue(obj, "PublicProperty", 5);

                Assert.IsTrue(result);
                Assert.AreEqual(5, obj.PublicProperty);
            }
        }

        [TestFixture]
        public class TheSetPropertyValueMethod
        {
            [TestCase]
            public void SetPropertyValue_NullInput()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.SetPropertyValue(null, "PublicProperty", 42));
            }

            [TestCase]
            public void SetPropertyValue_NotExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<PropertyNotFoundException>(() => PropertyHelper.SetPropertyValue(myPropertyHelperClass, "NotExistingProperty", 42));
            }

            [TestCase]
            public void SetPropertyValue_PrivateReadProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                PropertyHelper.SetPropertyValue(myPropertyHelperClass, "PrivateReadProperty", 42);
                Assert.AreEqual(42, myPropertyHelperClass.GetPrivateReadPropertyValue());
            }

            [TestCase]
            public void SetPropertyValue_PrivateWriteProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<CannotSetPropertyValueException>(() => PropertyHelper.SetPropertyValue(myPropertyHelperClass, "PrivateWriteProperty", 42));
            }

            [TestCase]
            public void SetPropertyValue_PublicProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                PropertyHelper.SetPropertyValue(myPropertyHelperClass, "PublicProperty", 42);
                Assert.AreEqual(42, myPropertyHelperClass.PublicProperty);
            }

            [TestCase]
            public void SetPropertyValue_StringValue()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                PropertyHelper.SetPropertyValue(myPropertyHelperClass, "StringValue", "FourtyTwo");
                Assert.AreEqual("FourtyTwo", myPropertyHelperClass.StringValue);
            }
        }
    }
}