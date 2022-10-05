namespace Catel.Tests.Reflection
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
                Assert.Throws<ArgumentNullException>(() => PropertyHelper.GetPropertyName<int>(null));
            }

            [TestCase]
            public void ThrowsNotSupportedExceptionForNoMemberAccessExpression()
            {
                Assert.Throws<NotSupportedException>(() => PropertyHelper.GetPropertyName(() => SomeMethod()));
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
