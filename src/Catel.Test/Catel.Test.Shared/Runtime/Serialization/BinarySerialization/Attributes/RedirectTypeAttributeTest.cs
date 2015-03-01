// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedirectTypeAttributeTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Runtime.Serialization
{
    using Catel.Runtime.Serialization.Binary;

    using NUnit.Framework;

    [TestFixture]
    public class RedirectTypeAttributeTest
    {
        #region Methods
        [TestCase]
        public void Constructor()
        {
            var attribute = new RedirectTypeAttribute("originalAssembly", "originalType");

            Assert.AreEqual("originalAssembly", attribute.OriginalAssemblyName);
            Assert.AreEqual("originalType", attribute.OriginalTypeName);
        }

        [TestCase]
        public void OriginalType()
        {
            var attribute = new RedirectTypeAttribute("Catel.Core.Old", "Catel.DataStuff.DataObjectBase");

            Assert.AreEqual("Catel.DataStuff.DataObjectBase, Catel.Core.Old", attribute.OriginalType);
        }

        [TestCase]
        public void TypeToLoad()
        {
            var attribute = new RedirectTypeAttribute("Catel.Core.Old", "Catel.DataStuff.DataObjectBase");

            attribute.NewAssemblyName = "Catel.Core";
            attribute.NewTypeName = "Catel.Data.DataObjectBase";

            Assert.AreEqual("Catel.Data.DataObjectBase, Catel.Core", attribute.TypeToLoad);
        }
        #endregion
    }
}

#endif