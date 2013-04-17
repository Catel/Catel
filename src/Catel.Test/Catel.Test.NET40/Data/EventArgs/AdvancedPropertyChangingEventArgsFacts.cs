// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvancedPropertyChangingEventArgsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class AdvancedPropertyChangingEventArgsFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void SetsPropertyNameCorrectly()
            {
                var e = new AdvancedPropertyChangingEventArgs("test");

                Assert.AreEqual("test", e.PropertyName);
            }

            [TestMethod]
            public void DefaultsCancelToFalse()
            {
                var e = new AdvancedPropertyChangingEventArgs("test");

                Assert.IsFalse(e.Cancel);
            }
        }
    }
}