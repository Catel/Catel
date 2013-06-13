// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelToModelAttributeFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.Attributes
{
    using Catel.MVVM;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ViewModelToModelAttributeFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void CorrectlySetsValueWithOneConstructorArgument()
            {
                var viewModelToModelAttribute = new ViewModelToModelAttribute("model");

                Assert.AreEqual("model", viewModelToModelAttribute.Model);
                Assert.AreEqual(string.Empty, viewModelToModelAttribute.Property);
            }

            [TestMethod]
            public void CorrectlySetsValueWithTwoConstructorArguments()
            {
                var viewModelToModelAttribute = new ViewModelToModelAttribute("model", "property");

                Assert.AreEqual("model", viewModelToModelAttribute.Model);
                Assert.AreEqual("property", viewModelToModelAttribute.Property);
            }
        }
    }
}