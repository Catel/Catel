// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelToModelAttributeFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

                Assert.AreEqual("model", viewModelToModelAttribute.Model);
                Assert.AreEqual(string.Empty, viewModelToModelAttribute.Property);
            }

            [TestCase]
            public void CorrectlySetsValueWithTwoConstructorArguments()
            {
                var viewModelToModelAttribute = new ViewModelToModelAttribute("model", "property");

                Assert.AreEqual("model", viewModelToModelAttribute.Model);
                Assert.AreEqual("property", viewModelToModelAttribute.Property);
            }
        }
    }
}