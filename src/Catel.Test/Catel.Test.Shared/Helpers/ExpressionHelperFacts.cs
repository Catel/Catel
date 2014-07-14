// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using System;
    using Data;

    using NUnit.Framework;

    public class ExpressionHelperFacts
    {
        [TestFixture]
        public class TheGetPropertyNameMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyExpression()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ExpressionHelper.GetPropertyName<object>(null));
            }

            [TestCase]
            public void ReturnsRightPropertyNameUsingExpression()
            {
                var iniEntry = new IniEntry();

                Assert.AreEqual("Group", ExpressionHelper.GetPropertyName(() => iniEntry.Group));
                Assert.AreEqual("Key", ExpressionHelper.GetPropertyName(() => iniEntry.Key));
                Assert.AreEqual("Value", ExpressionHelper.GetPropertyName(() => iniEntry.Value));
            }
        }

        [TestFixture]
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

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyExpression()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ExpressionHelper.GetOwner<object>(null));
            }

            [TestCase]
            public void ReturnsRightOwnerUsingExpression()
            {
                var owner = ExpressionHelper.GetOwner(() => MyProperty);

                Assert.IsTrue(ReferenceEquals(this, owner));
            }

            [TestCase]
            public void ReturnsRightOwnerWhenUsingAdditionalParentWithStringProperty()
            {
                var testModel = new TestModel();
                var owner = ExpressionHelper.GetOwner(() => testModel.StringProperty);

                Assert.IsTrue(ReferenceEquals(testModel, owner));
            }

            [TestCase]
            public void ReturnsRightOwnerWhenUsingAdditionalParentWithIntProperty()
            {
                var testModel = new TestModel();
                var owner = ExpressionHelper.GetOwner(() => testModel.IntProperty);

                Assert.IsTrue(ReferenceEquals(testModel, owner));
            }
        }
    }
}