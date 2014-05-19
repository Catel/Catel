// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using System;
    using Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ExpressionHelperFacts
    {
        [TestClass]
        public class TheGetPropertyNameMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullPropertyExpression()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ExpressionHelper.GetPropertyName<object>(null));
            }

            [TestMethod]
            public void ReturnsRightPropertyNameUsingExpression()
            {
                var iniEntry = new IniEntry();

                Assert.AreEqual("Group", ExpressionHelper.GetPropertyName(() => iniEntry.Group));
                Assert.AreEqual("Key", ExpressionHelper.GetPropertyName(() => iniEntry.Key));
                Assert.AreEqual("Value", ExpressionHelper.GetPropertyName(() => iniEntry.Value));
            }
        }

        [TestClass]
        public class TheGetOwnerMethod
        {
            public class TestModel
            {
                public string StringProperty { get; set; }

                public int IntProperty { get; set; }
            }

            /// <summary>
            ///   Test property to test owner.
            /// </summary>
            public string MyProperty { get; set; }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullPropertyExpression()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ExpressionHelper.GetOwner<object>(null));
            }

            [TestMethod]
            public void ReturnsRightOwnerUsingExpression()
            {
                var owner = ExpressionHelper.GetOwner(() => MyProperty);

                Assert.IsTrue(ReferenceEquals(this, owner));
            }

            [TestMethod]
            public void ReturnsRightOwnerWhenUsingAdditionalParentWithStringProperty()
            {
                var testModel = new TestModel();
                var owner = ExpressionHelper.GetOwner(() => testModel.StringProperty);

                Assert.IsTrue(ReferenceEquals(testModel, owner));
            }

            [TestMethod]
            public void ReturnsRightOwnerWhenUsingAdditionalParentWithIntProperty()
            {
                var testModel = new TestModel();
                var owner = ExpressionHelper.GetOwner(() => testModel.IntProperty);

                Assert.IsTrue(ReferenceEquals(testModel, owner));
            }
        }
    }
}