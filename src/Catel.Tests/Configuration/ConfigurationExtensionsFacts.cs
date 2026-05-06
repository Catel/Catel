namespace Catel.Tests.Configuration;

using Catel.Configuration;
using NUnit.Framework;

public class ConfigurationExtensionsFacts
{
    [TestFixture]
    public class The_IsConfigurationKey_Method
    {
        [Test]
        public void Returns_True_When_Key_Matches_Expected_Key()
        {
            var args = new ConfigurationChangedEventArgs(ConfigurationContainer.Local, "MyKey", null);

            Assert.That(args.IsConfigurationKey("MyKey"), Is.True);
        }

        [Test]
        public void Returns_False_When_Key_Does_Not_Match_Expected_Key()
        {
            var args = new ConfigurationChangedEventArgs(ConfigurationContainer.Local, "OtherKey", null);

            Assert.That(args.IsConfigurationKey("MyKey"), Is.False);
        }

        [Test]
        public void Returns_True_When_EventArgs_Key_Is_Null_Or_Whitespace_Indicating_Full_Scope_Update()
        {
            // A null or whitespace key in ConfigurationChangedEventArgs signals a full scope update,
            // so IsConfigurationKey should return true regardless of the expected key.
            Assert.That(((string)null!).IsConfigurationKey("MyKey"), Is.True);
            Assert.That("   ".IsConfigurationKey("MyKey"), Is.True);
        }

        [Test]
        public void Performs_Case_Insensitive_Comparison_Via_EventArgs()
        {
            var args = new ConfigurationChangedEventArgs(ConfigurationContainer.Local, "mykey", null);

            Assert.That(args.IsConfigurationKey("MyKey"), Is.True);
        }
    }
}
