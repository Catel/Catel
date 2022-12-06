namespace Catel.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsFacts
    {
        public class TheSplitByCamelCaseMethod
        {
            [TestCase(null, null)]
            [TestCase("ThisIsATest", "This is a test")]
            [TestCase("IsIncluded", "Is included")]
            [TestCase("PropertyName", "Property name")]
            public void SplitByCamelCase(string input, string expectedOutput)
            {
                var output = input.SplitCamelCase();

                Assert.AreEqual(expectedOutput, output);
            }

            [TestCase("this.hEllO something", "thishEllOsomething", false)]
            [TestCase("this.hEllO something", "thishellosomething", true)]
            [TestCase("tesTß", "tesT", false)]
            [TestCase("tesTß", "test", true)]
            [TestCase("tesT\\*&$", "tesT", false)]
            [TestCase("tesT\\*&$", "test", true)]
            public void GetSlug(string input, string expectedOutput, bool lowercase)
            {
                var output = input.GetSlug(makeLowercase: lowercase);

                Assert.AreEqual(expectedOutput, output);
            }

            [TestCase("HELLO", null, false)]
            [TestCase(null, "HELLO", false)]
            [TestCase("HELLO", "HELLO", true)]
            [TestCase("HELLO", "hello", true)]
            [TestCase("HELLO", "hello1", false)]
            public void EqualsIgnoreCase(string input1, string input2, bool expectedOutput)
            {
                var output = input1.EqualsIgnoreCase(input2);

                Assert.AreEqual(expectedOutput, output);
            }

            [TestCase("HELLO", null, false)]
            [TestCase(null, "HELLO", false)]
            [TestCase("HELLO", "HELL", true)]
            [TestCase("HELLO", "HELLO", true)]
            [TestCase("HELLO", "hello", true)]
            [TestCase("HELLO", "hello1", false)]
            public void ContainsIgnoreCase(string input1, string input2, bool expectedOutput)
            {
                var output = input1.ContainsIgnoreCase(input2);

                Assert.AreEqual(expectedOutput, output);
            }

            [TestCase("HELLO", null, false)]
            [TestCase(null, "HELLO", false)]
            [TestCase("HELLO", "HELL", true)]
            [TestCase("HELLO", "HELLO", true)]
            [TestCase("HELLO", "hello", true)]
            [TestCase("HELLO", "ello", false)]
            public void StartsWithIgnoreCase(string input1, string input2, bool expectedOutput)
            {
                var output = input1.StartsWithIgnoreCase(input2);

                Assert.AreEqual(expectedOutput, output);
            }

            [TestCase("HELLO", null, false)]
            [TestCase(null, "HELLO", false)]
            [TestCase("HELLO", "ELLO", true)]
            [TestCase("HELLO", "HELLO", true)]
            [TestCase("HELLO", "hello", true)]
            [TestCase("HELLO", "hell", false)]
            public void EndsWithIgnoreCase(string input1, string input2, bool expectedOutput)
            {
                var output = input1.EndsWithIgnoreCase(input2);

                Assert.AreEqual(expectedOutput, output);
            }

            [TestCase("HELLO", null, -1)]
            [TestCase(null, "HELLO", -1)]
            [TestCase("HELLO", "ELLO", 1)]
            [TestCase("HELLO", "HELLO", 0)]
            [TestCase("HELLO", "hello", 0)]
            [TestCase("HELLO", "hell", 0)]
            public void IndexOfIgnoreCase(string input1, string input2, int expectedOutput)
            {
                var output = input1.IndexOfIgnoreCase(input2);

                Assert.AreEqual(expectedOutput, output);
            }
        }
    }
}
