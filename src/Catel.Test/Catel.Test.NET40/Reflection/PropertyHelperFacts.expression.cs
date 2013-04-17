// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyHelperFacts.expression.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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
        [TestClass]
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

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullExpression()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => PropertyHelper.GetPropertyName<int>(null));
            }

            [TestMethod]
            public void ThrowsNotSupportedExceptionForNoMemberAccessExpression()
            {
                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => PropertyHelper.GetPropertyName(() => SomeMethod()));
            }

            [TestMethod]
            public void ReturnsPropertyNameForMemberExpressionWithoutNestedPropertiesWithoutAllowingNestedProperties()
            {
                var propertyName = PropertyHelper.GetPropertyName(() => TestProperty, false);

                Assert.AreEqual("TestProperty", propertyName);
            }

            [TestMethod]
            public void ReturnsPropertyNameForMemberExpressionWithNestedPropertiesWithoutAllowingNestedProperties()
            {
                var propertyName = PropertyHelper.GetPropertyName(() => TestProperty.SubClass.Id, false);

                Assert.AreEqual("Id", propertyName);
            }

            [TestMethod]
            public void ReturnsPropertyNameForMemberExpressionWithoutNestedPropertiesWithAllowingNestedProperties()
            {
                var propertyName = PropertyHelper.GetPropertyName(() => TestProperty, true);

                Assert.AreEqual("TestProperty", propertyName);
            }

            [TestMethod]
            public void ReturnsPropertyNameForMemberExpressionWithNestedPropertiesWithAllowingNestedProperties()
            {
                var propertyName = PropertyHelper.GetPropertyName(() => TestProperty.SubClass.Id, true);

                Assert.AreEqual("TestProperty.SubClass.Id", propertyName);
            }
        }
    }
}