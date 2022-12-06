namespace Catel.Tests.Text
{
    using Catel.Text;
    using System.Text;
    using NUnit.Framework;

    public class StringBuilderExtensionFacts
    {
        [TestFixture]
        public class TheAppendLineMethod
        {
            [TestCase]
            public void CorrectlyAppendsLineWithFormatting()
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine("test with {0} {1}", "formatting", 1);

                Assert.AreEqual("test with formatting 1\r\n", stringBuilder.ToString());
            }
        }
    }
}