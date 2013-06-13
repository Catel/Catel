// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedirectTypeAttributeTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Runtime.Serialization
{
    using Catel.Runtime.Serialization;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class RedirectTypeAttributeTest
    {
        #region Methods
        [TestMethod]
        public void Constructor()
        {
            RedirectTypeAttribute attribute = new RedirectTypeAttribute("originalAssembly", "originalType");

            Assert.AreEqual("originalAssembly", attribute.OriginalAssemblyName);
            Assert.AreEqual("originalType", attribute.OriginalTypeName);
        }

        [TestMethod]
        public void OriginalType()
        {
            RedirectTypeAttribute attribute = new RedirectTypeAttribute("Catel.Core.Old", "Catel.DataStuff.DataObjectBase");

            Assert.AreEqual("Catel.DataStuff.DataObjectBase, Catel.Core.Old", attribute.OriginalType);
        }

        [TestMethod]
        public void TypeToLoad()
        {
            RedirectTypeAttribute attribute = new RedirectTypeAttribute("Catel.Core.Old", "Catel.DataStuff.DataObjectBase");

            attribute.NewAssemblyName = "Catel.Core";
            attribute.NewTypeName = "Catel.Data.DataObjectBase";

            Assert.AreEqual("Catel.Data.DataObjectBase, Catel.Core", attribute.TypeToLoad);
        }
        #endregion
    }
}