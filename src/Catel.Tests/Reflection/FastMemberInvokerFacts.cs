namespace Catel.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Reflection;
    using NUnit.Framework;

    public class FastMemberInvokerFacts
    {
        public class TestClassWithRegularMembers
        {
            public string StringField;

            public string StringProperty { get; set; }
        }

        [TestFixture]
        public class TheTryGetFieldValueMethod
        {
            [TestCase]
            public void ReturnsStringField()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers
                {
                    StringField = "John"
                };

                Assert.IsTrue(fastMemberInvoker.TryGetFieldValue<string>(testClass, nameof(TestClassWithRegularMembers.StringField), out var stringValue));
                Assert.AreEqual("John", stringValue);
            }

            [TestCase]
            public void ReturnsFalseForNonExistingField()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers
                {
                    StringField = "John"
                };

                Assert.IsFalse(fastMemberInvoker.TryGetFieldValue<string>(testClass, "NonExistingField", out var stringValue));
            }
        }

        [TestFixture]
        public class TheTryGetPropertyValueMethod
        {
            [TestCase]
            public void ReturnsStringProperty()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers
                {
                    StringProperty = "John"
                };

                Assert.IsTrue(fastMemberInvoker.TryGetPropertyValue<string>(testClass, nameof(TestClassWithRegularMembers.StringProperty), out var stringValue));
                Assert.AreEqual("John", stringValue);
            }

            [TestCase]
            public void ReturnsFalseForNonExistingField()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers
                {
                    StringProperty = "John"
                };

                Assert.IsFalse(fastMemberInvoker.TryGetPropertyValue<string>(testClass, "NonExistingProperty", out var stringValue));
            }
        }

        [TestFixture]
        public class TheSetFieldValueMethod
        {
            [TestCase]
            public void UpdatesStringField()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers();

                Assert.IsTrue(fastMemberInvoker.SetFieldValue<string>(testClass, nameof(TestClassWithRegularMembers.StringField), "John"));
                Assert.AreEqual("John", testClass.StringField);
            }

            [TestCase]
            public void ReturnsFalseForNonExistingField()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers();

                Assert.IsFalse(fastMemberInvoker.SetFieldValue<string>(testClass, "NonExistingField", "John"));
            }
        }

        [TestFixture]
        public class TheSetPropertyValueMethod
        {
            [TestCase]
            public void UpdatesStringProperty()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers();

                Assert.IsTrue(fastMemberInvoker.SetPropertyValue<string>(testClass, nameof(TestClassWithRegularMembers.StringProperty), "John"));
                Assert.AreEqual("John", testClass.StringProperty);
            }

            [TestCase]
            public void ReturnsFalseForNonExistingProperty()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers();

                Assert.IsFalse(fastMemberInvoker.SetPropertyValue<string>(testClass, "NonExistingProperty", "John"));
            }
        }
    }
}
