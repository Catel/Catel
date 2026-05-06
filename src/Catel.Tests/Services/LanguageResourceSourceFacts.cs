namespace Catel.Tests.Services;

using Catel.Services;
using NUnit.Framework;

public class LanguageResourceSourceFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Sets_Properties_From_AssemblyName_Only()
        {
            var source = new LanguageResourceSource("MyAssembly");

            Assert.That(source.AssemblyName, Is.EqualTo("MyAssembly"));
            Assert.That(source.NamespaceName, Is.EqualTo("MyAssembly.Properties"));
            Assert.That(source.ResourceFileName, Is.EqualTo("Resources"));
        }

        [Test]
        public void Sets_Properties_From_AssemblyName_And_Namespace()
        {
            var source = new LanguageResourceSource("MyAssembly", "MyAssembly.Lang");

            Assert.That(source.AssemblyName, Is.EqualTo("MyAssembly"));
            Assert.That(source.NamespaceName, Is.EqualTo("MyAssembly.Lang"));
            Assert.That(source.ResourceFileName, Is.EqualTo("Resources"));
        }

        [Test]
        public void Sets_All_Properties_From_Full_Constructor()
        {
            var source = new LanguageResourceSource("MyAssembly", "MyAssembly.Lang", "Strings");

            Assert.That(source.AssemblyName, Is.EqualTo("MyAssembly"));
            Assert.That(source.NamespaceName, Is.EqualTo("MyAssembly.Lang"));
            Assert.That(source.ResourceFileName, Is.EqualTo("Strings"));
        }
    }

    [TestFixture]
    public class The_GetSource_Method
    {
        [Test]
        public void Returns_Correct_Format()
        {
            var source = new LanguageResourceSource("MyAssembly", "MyAssembly.Lang", "Strings");

            var result = source.GetSource();

            Assert.That(result, Is.EqualTo("MyAssembly.Lang.Strings, MyAssembly"));
        }
    }
}
