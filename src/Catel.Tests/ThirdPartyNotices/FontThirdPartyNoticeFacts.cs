namespace Catel.Tests.ThirdPartyNotices;

using Catel.ThirdPartyNotices;
using NUnit.Framework;

public class FontThirdPartyNoticeFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_Provided_Values()
        {
            var notice = new FontThirdPartyNotice("Open Sans", "https://example.com");

            Assert.That(notice.Title, Is.EqualTo("Open Sans"));
            Assert.That(notice.Url, Is.EqualTo("https://example.com"));
            Assert.That(notice.Content, Is.EqualTo(string.Empty));
        }
    }
}
