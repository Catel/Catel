namespace Catel.Tests.ThirdPartyNotices;

using Catel.ThirdPartyNotices;
using NUnit.Framework;

public class ThirdPartyNoticeFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Initializes_With_Empty_Values()
        {
            var notice = new ThirdPartyNotice();

            Assert.That(notice.Title, Is.EqualTo(string.Empty));
            Assert.That(notice.Content, Is.EqualTo(string.Empty));
            Assert.That(notice.Url, Is.EqualTo(string.Empty));
        }
    }
}
