namespace Catel.Tests.MVVM.ViewModels.Attributes
{
    using Catel.MVVM;

    using NUnit.Framework;

    public class ViewModelToModelAttributeFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void CorrectlySetsValueWithOneConstructorArgument()
            {
                var viewModelToModelAttribute = new ViewModelToModelAttribute("model");

                Assert.That(viewModelToModelAttribute.Model, Is.EqualTo("model"));
                Assert.That(viewModelToModelAttribute.Property, Is.EqualTo(string.Empty));
            }

            [TestCase]
            public void CorrectlySetsValueWithTwoConstructorArguments()
            {
                var viewModelToModelAttribute = new ViewModelToModelAttribute("model", "property");

                Assert.That(viewModelToModelAttribute.Model, Is.EqualTo("model"));
                Assert.That(viewModelToModelAttribute.Property, Is.EqualTo("property"));
            }
        }
    }
}