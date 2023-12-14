namespace Catel.Tests.Reflection
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel.Reflection;
    using Catel.Tests.Reflection.Models;
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

                Assert.That(fastMemberInvoker.TryGetPropertyValue<RecordDetailItemValue>(recordDetailItem, nameof(recordDetailItem.ComparedValue1), out var item1), Is.True);
                Assert.That(ReferenceEquals(recordDetailItem.ComparedValue1, item1), Is.True);

                Assert.That(fastMemberInvoker.TryGetPropertyValue<RecordDetailItemValue>(recordDetailItem, nameof(recordDetailItem.ComparedValue2), out var item2), Is.True);
                Assert.That(ReferenceEquals(recordDetailItem.ComparedValue2, item2), Is.True);
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

                Assert.That(fastMemberInvoker.TryGetPropertyValue<bool>(recordDetailItem, nameof(recordDetailItem.BoolValue), out var boolValue), Is.True);
                Assert.That(boolValue, Is.True);
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

                Assert.That(fastMemberInvoker.TryGetPropertyValue<int>(recordDetailItem, nameof(recordDetailItem.IntValue), out var intValue), Is.True);
                Assert.That(intValue, Is.EqualTo(42));
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

                Assert.That(fastMemberInvoker.TryGetFieldValue<string>(testClass, nameof(TestClassWithRegularMembers.StringField), out var stringValue), Is.True);
                Assert.That(stringValue, Is.EqualTo("John"));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingField()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers
                {
                    StringField = "John"
                };

                Assert.That(fastMemberInvoker.TryGetFieldValue<string>(testClass, "NonExistingField", out var stringValue), Is.False);
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

                Assert.That(fastMemberInvoker.TryGetPropertyValue<string>(testClass, nameof(TestClassWithRegularMembers.StringProperty), out var stringValue), Is.True);
                Assert.That(stringValue, Is.EqualTo("John"));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingProperty()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers
                {
                    StringProperty = "John"
                };

                Assert.That(fastMemberInvoker.TryGetPropertyValue<string>(testClass, "NonExistingProperty", out var stringValue), Is.False);
            }
        }

        [TestFixture]
        public class TheTrySetFieldValueMethod
        {
            [TestCase]
            public void UpdatesStringField()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers();

                Assert.That(fastMemberInvoker.TrySetFieldValue<string>(testClass, nameof(TestClassWithRegularMembers.StringField), "John"), Is.True);
                Assert.That(testClass.StringField, Is.EqualTo("John"));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingField()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers();

                Assert.That(fastMemberInvoker.TrySetFieldValue<string>(testClass, "NonExistingField", "John"), Is.False);
            }
        }

        [TestFixture]
        public class TheTrySetPropertyValueMethod
        {
            [TestCase]
            public void UpdatesStringProperty()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers();

                Assert.That(fastMemberInvoker.TrySetPropertyValue<string>(testClass, nameof(TestClassWithRegularMembers.StringProperty), "John"), Is.True);
                Assert.That(testClass.StringProperty, Is.EqualTo("John"));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingProperty()
            {
                var fastMemberInvoker = new FastMemberInvoker<TestClassWithRegularMembers>();

                var testClass = new TestClassWithRegularMembers();

                Assert.That(fastMemberInvoker.TrySetPropertyValue<string>(testClass, "NonExistingProperty", "John"), Is.False);
            }
        }
    }
}
