namespace Catel.Tests
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
                Assert.Throws<ArgumentNullException>(() => ExpressionHelper.GetPropertyName<object>(null));
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
                public TestModel()
                {
                    InnerModel = new InnerTestModel();
                }

                public string StringProperty { get; set; }

                public int IntProperty { get; set; }

                public InnerTestModel InnerModel { get; private set; }
            }

            public class InnerTestModel
            {
                public string InnerProperty { get; set; }
            }

            public string MyProperty { get; set; }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullPropertyExpression()
            {
                Assert.Throws<ArgumentNullException>(() => ExpressionHelper.GetOwner<object>(null));
            }

            [TestCase]
            public void ReturnsRightOwnerUsingExpression()
            {
                var owner = ExpressionHelper.GetOwner(() => MyProperty);

                Assert.IsTrue(ReferenceEquals(this, owner));
            }

            [TestCase]
            public void ReturnsRightOwnerWhenUsingAdditionalParent()
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

            [TestCase]
            public void ReturnsRightOwnerWhenUsingInnerModel()
            {
                var testModel = new TestModel();
                var owner = ExpressionHelper.GetOwner(() => testModel.InnerModel.InnerProperty);

                Assert.IsTrue(ReferenceEquals(testModel.InnerModel, owner));
            }
        }
    }
}
