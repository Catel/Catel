// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System;
    using Catel.Reflection;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

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

        [TestClass]
        public class TheIsPropertyAvailableMethod
        {
            [TestMethod]
            public void IsPropertyAvailable_NullInput()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.IsPropertyAvailable(null, "PublicProperty"));
            }

            [TestMethod]
            public void IsPropertyAvailable_NotExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.AreEqual(false, PropertyHelper.IsPropertyAvailable(myPropertyHelperClass, "NotExistingProperty"));
            }

            [TestMethod]
            public void IsPropertyAvailable_ExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.AreEqual(true, PropertyHelper.IsPropertyAvailable(myPropertyHelperClass, "PublicProperty"));
            }
        }

        [TestClass]
        public class TheTryGetPropertyValueMethod
        {
            [TestMethod]
            public void TryGetPropertyValue_ObjectNull()
            {
                object value;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.TryGetPropertyValue(null, "property", out value));
            }

            [TestMethod]
            public void TryGetPropertyValue_PropertyNameNull()
            {
                object value;
                var obj = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => PropertyHelper.TryGetPropertyValue(obj, null, out value));
            }

            [TestMethod]
            public void TryGetPropertyValue_NonExistingProperty()
            {
                object value;
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TryGetPropertyValue(obj, "Non-existing property", out value);

                Assert.IsFalse(result);
                Assert.AreEqual(null, value);
            }

            [TestMethod]
            public void TryGetPropertyValue_ExistingProperty()
            {
                object value;
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TryGetPropertyValue(obj, "PublicProperty", out value);

                Assert.IsTrue(result);
                Assert.AreEqual(1, value);
            }
        }

        [TestClass]
        public class TheGetPropertyValueMethod
        {
            [TestMethod]
            public void GetPropertyValue_NullInput()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.GetPropertyValue(null, "PublicProperty"));
            }

            [TestMethod]
            public void GetPropertyValue_NotExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<PropertyNotFoundException>(() => PropertyHelper.GetPropertyValue(myPropertyHelperClass, "NotExistingProperty"));
            }

            [TestMethod]
            public void GetPropertyValue_PrivateReadProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<CannotGetPropertyValueException>(() => PropertyHelper.GetPropertyValue(myPropertyHelperClass, "PrivateReadProperty"));
            }

            [TestMethod]
            public void GetPropertyValue_PrivateWriteProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                Assert.AreEqual(4, PropertyHelper.GetPropertyValue<int>(myPropertyHelperClass, "PrivateWriteProperty"));
            }

            [TestMethod]
            public void GetPropertyValue_PublicProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                myPropertyHelperClass.PublicProperty = 42;
                Assert.AreEqual(42, PropertyHelper.GetPropertyValue<int>(myPropertyHelperClass, "PublicProperty"));
            }

            [TestMethod]
            public void GetPropertyValue_StringValue()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                myPropertyHelperClass.StringValue = "FourtyTwo";
                Assert.AreEqual("FourtyTwo", PropertyHelper.GetPropertyValue<string>(myPropertyHelperClass, "StringValue"));
            }
        }

        [TestClass]
        public class TrySetPropertyValueMethod
        {
            [TestMethod]
            public void TrySetPropertyValue_ObjectNull()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.TrySetPropertyValue(null, "property", null));
            }

            [TestMethod]
            public void TrySetPropertyValue_PropertyNameNull()
            {
                var obj = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => PropertyHelper.TrySetPropertyValue(obj, null, null));
            }

            [TestMethod]
            public void TrySetPropertyValue_NonExistingProperty()
            {
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TrySetPropertyValue(obj, "Non-existing property", null);

                Assert.IsFalse(result);
            }

            [TestMethod]
            public void TrySetPropertyValue_ExistingProperty()
            {
                var obj = new MyPropertyHelperClass();

                var result = PropertyHelper.TrySetPropertyValue(obj, "PublicProperty", 5);

                Assert.IsTrue(result);
                Assert.AreEqual(5, obj.PublicProperty);
            }
        }

        [TestClass]
        public class TheSetPropertyValueMethod
        {
            [TestMethod]
            public void SetPropertyValue_NullInput()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.SetPropertyValue(null, "PublicProperty", 42));
            }

            [TestMethod]
            public void SetPropertyValue_NotExistingProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<PropertyNotFoundException>(() => PropertyHelper.SetPropertyValue(myPropertyHelperClass, "NotExistingProperty", 42));
            }

            [TestMethod]
            public void SetPropertyValue_PrivateReadProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                PropertyHelper.SetPropertyValue(myPropertyHelperClass, "PrivateReadProperty", 42);
                Assert.AreEqual(42, myPropertyHelperClass.GetPrivateReadPropertyValue());
            }

            [TestMethod]
            public void SetPropertyValue_PrivateWriteProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                ExceptionTester.CallMethodAndExpectException<CannotSetPropertyValueException>(() => PropertyHelper.SetPropertyValue(myPropertyHelperClass, "PrivateWriteProperty", 42));
            }

            [TestMethod]
            public void SetPropertyValue_PublicProperty()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                PropertyHelper.SetPropertyValue(myPropertyHelperClass, "PublicProperty", 42);
                Assert.AreEqual(42, myPropertyHelperClass.PublicProperty);
            }

            [TestMethod]
            public void SetPropertyValue_StringValue()
            {
                var myPropertyHelperClass = new MyPropertyHelperClass();
                PropertyHelper.SetPropertyValue(myPropertyHelperClass, "StringValue", "FourtyTwo");
                Assert.AreEqual("FourtyTwo", myPropertyHelperClass.StringValue);
            }
        }
    }
}