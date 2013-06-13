// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAttributeFacts.cs" company="Catel development team">
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

    public class ModelAttributeFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void CorrectlySetsDefaultValues()
            {
                var modelAttribute = new ModelAttribute();

                Assert.AreEqual(true, modelAttribute.SupportIEditableObject);
            }
        }
    }
}