// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAttributeFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.Attributes
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

                Assert.AreEqual(true, modelAttribute.SupportIEditableObject);
            }

            [TestCase]
            public void CorrectlySetsDefault_SupportValidation()
            {
                var modelAttribute = new ModelAttribute();

                Assert.AreEqual(true, modelAttribute.SupportValidation);
            }
        }
    }
}