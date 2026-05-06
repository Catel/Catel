namespace Catel.Tests.Configuration;

using Catel.Configuration;
using NUnit.Framework;

public class ConfigurationChangedEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Sets_Properties_Correctly()
        {
            var args = new ConfigurationChangedEventArgs(ConfigurationContainer.Local, "MyKey", "NewValue");

            Assert.That(args.Container, Is.EqualTo(ConfigurationContainer.Local));
            Assert.That(args.Key, Is.EqualTo("MyKey"));
            Assert.That(args.NewValue, Is.EqualTo("NewValue"));
        }

        [Test]
        public void Allows_Null_NewValue()
        {
            var args = new ConfigurationChangedEventArgs(ConfigurationContainer.Roaming, "MyKey", null);

            Assert.That(args.NewValue, Is.Null);
        }
    }
}
