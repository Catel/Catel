namespace Catel.Tests.Data
{
    using System.Linq;
    using Catel.Data;
    using NUnit.Framework;

    [TestFixture]
    public class SuspensionContextFacts
    {
        [TestCase]
        public void CorrectlyAddsProperties()
        {
            var suspensionContext = new SuspensionContext();

            suspensionContext.Add(null);
            suspensionContext.Add("A");
            suspensionContext.Add("B");

            var items = suspensionContext.Properties.ToList();

            Assert.AreEqual(string.Empty, items[0]);
            Assert.AreEqual("A", items[1]);
            Assert.AreEqual("B", items[2]);
        }

        [TestCase]
        public void PreventsDuplicates()
        {
            var suspensionContext = new SuspensionContext();

            suspensionContext.Add(null);
            suspensionContext.Add(string.Empty);
            suspensionContext.Add("A");
            suspensionContext.Add("A");

            var items = suspensionContext.Properties.ToList();

            Assert.AreEqual(2, items.Count);
            Assert.AreEqual(string.Empty, items[0]);
            Assert.AreEqual("A", items[1]);
        }
    }
}