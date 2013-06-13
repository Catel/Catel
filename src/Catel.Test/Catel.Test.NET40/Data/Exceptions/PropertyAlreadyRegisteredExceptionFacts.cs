// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyAlreadyRegisteredExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data.Exceptions
{
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class PropertyAlreadyRegisteredExceptionFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void SetsValuesCorrectly()
            {
                var exception = new PropertyAlreadyRegisteredException("PropertyName", typeof(string));
                Assert.AreEqual("PropertyName", exception.PropertyName);
                Assert.AreEqual(typeof(string), exception.PropertyType);
            }
        }
    }
}