namespace Catel.Tests.ThirdPartyNotices;

using System.IO;
using Catel.ThirdPartyNotices;
using NUnit.Framework;

public class FileBasedThirdPartyNoticeFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Reads_File_Content_From_Constructor()
        {
            var fileName = Path.GetTempFileName();

            try
            {
                File.WriteAllText(fileName, "test content");

                var notice = new FileBasedThirdPartyNotice("title", "url", fileName);

                Assert.That(notice.Title, Is.EqualTo("title"));
                Assert.That(notice.Url, Is.EqualTo("url"));
                Assert.That(notice.Content, Is.EqualTo("test content"));
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }
    }
}
