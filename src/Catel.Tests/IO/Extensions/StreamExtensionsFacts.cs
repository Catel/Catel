namespace Catel.Tests.IO
{
    using Catel.IO;
    using System.IO;
    using NUnit.Framework;

    public class StreamExtensionsFacts
    {
        [TestFixture]
        public class TheGetUtf8StringMethod
        {
            [TestCase("simplestring")]
            [TestCase("string with spaces")]
            public void ReturnsRightString(string input)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(memoryStream))
                    {
                        streamWriter.Write(input);
                        streamWriter.Flush();

                        var output = memoryStream.GetUtf8String();

                        Assert.AreEqual(input, output);
                    }
                }
            }
        }
    }
}
