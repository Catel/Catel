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

                Assert.That(ExpressionHelper.GetPropertyName(() => iniEntry.Group), Is.EqualTo("Group"));
                Assert.That(ExpressionHelper.GetPropertyName(() => iniEntry.Key), Is.EqualTo("Key"));
                Assert.That(ExpressionHelper.GetPropertyName(() => iniEntry.Value), Is.EqualTo("Value"));
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

                Assert.That(ReferenceEquals(this, owner), Is.True);
            }

            [TestCase]
            public void ReturnsRightOwnerWhenUsingAdditionalParent()
            {
                var testModel = new TestModel();
                var owner = ExpressionHelper.GetOwner(() => testModel.StringProperty);

                Assert.That(ReferenceEquals(testModel, owner), Is.True);
            }

            [TestCase]
            public void ReturnsRightOwnerWhenUsingAdditionalParentWithIntProperty()
            {
                var testModel = new TestModel();
                var owner = ExpressionHelper.GetOwner(() => testModel.IntProperty);

                Assert.That(ReferenceEquals(testModel, owner), Is.True);
            }

            [TestCase]
            public void ReturnsRightOwnerWhenUsingInnerModel()
            {
                var testModel = new TestModel();
                var owner = ExpressionHelper.GetOwner(() => testModel.InnerModel.InnerProperty);

                Assert.That(ReferenceEquals(testModel.InnerModel, owner), Is.True);
            }
        }
    }
}
