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
                var model = new PersonTestModel();

                Assert.That(ExpressionHelper.GetPropertyName(() => model.FirstName), Is.EqualTo(nameof(PersonTestModel.FirstName)));
                Assert.That(ExpressionHelper.GetPropertyName(() => model.LastName), Is.EqualTo(nameof(PersonTestModel.LastName)));
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
