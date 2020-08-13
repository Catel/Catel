namespace Catel.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Reflection;
    using Catel.Tests.Reflection.Models;
    using Catel.Threading;
    using NUnit.Framework;

    public class FastMemberInvokerFacts
    {
        public class TestClassWithRegularMembers
        {
            public string StringField;

            public string StringProperty { get; set; }

            public int Int01 { get; set; }
            public int Int02 { get; set; }
            public int Int03 { get; set; }
            public int Int04 { get; set; }
            public int Int05 { get; set; }
            public int Int06 { get; set; }
            public int Int07 { get; set; }
            public int Int08 { get; set; }
            public int Int09 { get; set; }
            public int Int10 { get; set; }
            public int Int11 { get; set; }
            public int Int12 { get; set; }
            public int Int13 { get; set; }
            public int Int14 { get; set; }
            public int Int15 { get; set; }
            public int Int16 { get; set; }
            public int Int17 { get; set; }
            public int Int18 { get; set; }
            public int Int19 { get; set; }
            public int Int20 { get; set; }
            public int Int21 { get; set; }
            public int Int22 { get; set; }
            public int Int23 { get; set; }
            public int Int24 { get; set; }
            public int Int25 { get; set; }
            public int Int26 { get; set; }
            public int Int27 { get; set; }
            public int Int28 { get; set; }
            public int Int29 { get; set; }
        }

        [TestFixture]
        public class IntegrationTests
        {
            [TestCase]
            public void TestThreadSafety()
            {
                // https://github.com/Catel/Catel/issues/1518

                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();
                var instance = new TestClassWithRegularMembers();

                var tasks = new List<Task>();

                for (int i = 0; i < 30; i++)
                {
                    tasks.Add(Task.Run(() => fastMemberInvoker.TryGetPropertyValue(instance, $"Int{i + 1:D2}", out int _)));
                }

                Task.WaitAll(tasks.ToArray());
            }

            [TestCase]
            public void CorrectlyGetsAndSetsReferenceValues()
            {
                var fastMemberInvoker = new FastMemberInvoker<RecordDetailItem>();

                var recordDetailItem = new RecordDetailItem
                {
                    Name = "test",
                    Value = "A"
                };

                recordDetailItem.ComparedValue1 = new RecordDetailItemValue(recordDetailItem)
                {
                    Value = "B"
                };

                recordDetailItem.ComparedValue2 = new RecordDetailItemValue(recordDetailItem)
                {
                    Value = "C"
                };

                Assert.IsTrue(fastMemberInvoker.TryGetPropertyValue<RecordDetailItemValue>(recordDetailItem, nameof(recordDetailItem.ComparedValue1), out var item1));
                Assert.IsTrue(ReferenceEquals(recordDetailItem.ComparedValue1, item1));

                Assert.IsTrue(fastMemberInvoker.TryGetPropertyValue<RecordDetailItemValue>(recordDetailItem, nameof(recordDetailItem.ComparedValue2), out var item2));
                Assert.IsTrue(ReferenceEquals(recordDetailItem.ComparedValue2, item2));
            }

            [TestCase]
            public void CorrectlyGetsAndSetsBooleanValues()
            {
                var fastMemberInvoker = new FastMemberInvoker<RecordDetailItem>();

                var recordDetailItem = new RecordDetailItem
                {
                    Name = "test",
                    Value = "A",
                    BoolValue = true
                };

                Assert.IsTrue(fastMemberInvoker.TryGetPropertyValue<bool>(recordDetailItem, nameof(recordDetailItem.BoolValue), out var boolValue));
                Assert.IsTrue(boolValue);
            }

            [TestCase]
            public void CorrectlyGetsAndSetsIntValues()
            {
                var fastMemberInvoker = new FastMemberInvoker<RecordDetailItem>();

                var recordDetailItem = new RecordDetailItem
                {
                    Name = "test",
                    Value = "A",
                    IntValue = 42
                };

                Assert.IsTrue(fastMemberInvoker.TryGetPropertyValue<int>(recordDetailItem, nameof(recordDetailItem.IntValue), out var intValue));
                Assert.AreEqual(42, intValue);
            }
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
            public void ReturnsFalseForNonExistingProperty()
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
