namespace Catel.Tests.Data.Adapters
{
    using Catel.Data;
    using NUnit.Framework;

    public class ExpressionTreeObjectAdapterFacts
    {
        public class TestClassWithRegularMembers
        {
            public string StringField;

            public string StringProperty { get; set; }
        }

        [TestFixture]
        public class TheTrySetValueMethod
        {
            [TestCase]
            public void SetsCatelModelPropertyValue()
            {
                var adapter = new ExpressionTreeObjectAdapter();
                var model = new PersonTestModel();

                Assert.That(adapter.TrySetMemberValue(model, nameof(PersonTestModel.FirstName), "John"), Is.True);
                Assert.That(model.FirstName, Is.EqualTo("John"));
            }

            [TestCase]
            public void SetsRegularPropertyValue()
            {
                var adapter = new ExpressionTreeObjectAdapter();
                var model = new TestClassWithRegularMembers();

                Assert.That(adapter.TrySetMemberValue(model, nameof(TestClassWithRegularMembers.StringProperty), "John"), Is.True);
                Assert.That(model.StringProperty, Is.EqualTo("John"));
            }

            [TestCase]
            public void SetsFieldValue()
            {
                var adapter = new ExpressionTreeObjectAdapter();
                var model = new TestClassWithRegularMembers();

                Assert.That(adapter.TrySetMemberValue(model, nameof(TestClassWithRegularMembers.StringField), "John"), Is.True);
                Assert.That(model.StringField, Is.EqualTo("John"));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingMember()
            {
                var adapter = new ExpressionTreeObjectAdapter();
                var model = new TestClassWithRegularMembers();

                Assert.That(adapter.TrySetMemberValue(model, "NotExistingMember", "John"), Is.False);
            }
        }

        [TestFixture]
        public class TheTryGetValueMethod
        {
            [TestCase]
            public void GetsCatelModelPropertyValue()
            {
                var adapter = new ExpressionTreeObjectAdapter();
                var model = new PersonTestModel
                {
                    FirstName = "John"
                };

                string value = string.Empty;

                Assert.That(adapter.TryGetMemberValue(model, nameof(PersonTestModel.FirstName), out value), Is.True);
                Assert.That(model.FirstName, Is.EqualTo("John"));
            }

            [TestCase]
            public void GetsRegularPropertyValue()
            {
                var adapter = new ExpressionTreeObjectAdapter();
                var model = new TestClassWithRegularMembers
                {
                    StringProperty = "John"
                };

                string value = string.Empty;

                Assert.That(adapter.TryGetMemberValue(model, nameof(TestClassWithRegularMembers.StringProperty), out value), Is.True);
                Assert.That(value, Is.EqualTo("John"));
            }

            [TestCase]
            public void GetsFieldValue()
            {
                var adapter = new ExpressionTreeObjectAdapter();
                var model = new TestClassWithRegularMembers
                {
                    StringField = "John"
                };

                string value = string.Empty;

                Assert.That(adapter.TryGetMemberValue(model, nameof(TestClassWithRegularMembers.StringField), out value), Is.True);
                Assert.That(value, Is.EqualTo("John"));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingMember()
            {
                var adapter = new ExpressionTreeObjectAdapter();
                var model = new TestClassWithRegularMembers();
                string value = string.Empty;

                Assert.That(adapter.TryGetMemberValue(model, "NotExistingMember", out value), Is.False);
            }
        }
    }
}
