// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyHelperFacts.expression.cs" company="Catel development team">
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
        [TestFixture]
        public class TheGetPropertyNameWithExpressionMethod
        {
            #region TestClasses
            public TheGetPropertyNameWithExpressionMethod()
            {
                TestProperty = new A();
            }

            public A TestProperty { get; set; }

            public class A
            {
                public A()
                {
                    Id = UniqueIdentifierHelper.GetUniqueIdentifier<A>();
                    SubClass = new B();
                }

                public int Id { get; set; }

                public B SubClass { get; set; }
            }

            public class B
            {
                public B()
                {
                    Id = UniqueIdentifierHelper.GetUniqueIdentifier<B>();
                }

                public int Id { get; set; }
            }

            public int SomeMethod()
            {
                return 0;
            }
            #endregion

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullExpression()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.GetPropertyName<int>(null));
            }

            [TestCase]
            public void ThrowsNotSupportedExceptionForNoMemberAccessExpression()
            {
                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => PropertyHelper.GetPropertyName(() => SomeMethod()));
            }

            [TestCase]
            public void ReturnsPropertyNameForMemberExpressionWithoutNestedPropertiesWithoutAllowingNestedProperties()
            {
                var propertyName = PropertyHelper.GetPropertyName(() => TestProperty, false);

                Assert.AreEqual("TestProperty", propertyName);
            }

            [TestCase]
            public void ReturnsPropertyNameForMemberExpressionWithNestedPropertiesWithoutAllowingNestedProperties()
            {
                var propertyName = PropertyHelper.GetPropertyName(() => TestProperty.SubClass.Id, false);

                Assert.AreEqual("Id", propertyName);
            }

            [TestCase]
            public void ReturnsPropertyNameForMemberExpressionWithoutNestedPropertiesWithAllowingNestedProperties()
            {
                var propertyName = PropertyHelper.GetPropertyName(() => TestProperty, true);

                Assert.AreEqual("TestProperty", propertyName);
            }

            [TestCase]
            public void ReturnsPropertyNameForMemberExpressionWithNestedPropertiesWithAllowingNestedProperties()
            {
                var propertyName = PropertyHelper.GetPropertyName(() => TestProperty.SubClass.Id, true);

                Assert.AreEqual("TestProperty.SubClass.Id", propertyName);
            }
        }
    }
}