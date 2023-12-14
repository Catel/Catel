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

            Assert.That(items[0], Is.EqualTo(string.Empty));
            Assert.That(items[1], Is.EqualTo("A"));
            Assert.That(items[2], Is.EqualTo("B"));
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

            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items[0], Is.EqualTo(string.Empty));
            Assert.That(items[1], Is.EqualTo("A"));
        }
    }
}