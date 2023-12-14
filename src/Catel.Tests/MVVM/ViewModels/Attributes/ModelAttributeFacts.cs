namespace Catel.Tests.MVVM.ViewModels.Attributes
{
    using Catel.MVVM;

    using NUnit.Framework;

    public class ModelAttributeFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void CorrectlySetsDefaultValue_SupportIEditableObject()
            {
                var modelAttribute = new ModelAttribute();

                Assert.That(modelAttribute.SupportIEditableObject, Is.EqualTo(true));
            }

            [TestCase]
            public void CorrectlySetsDefault_SupportValidation()
            {
                var modelAttribute = new ModelAttribute();

                Assert.That(modelAttribute.SupportValidation, Is.EqualTo(true));
            }
        }
    }
}