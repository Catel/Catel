// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyNotNullableExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
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

    public class PropertyNotNullableExceptionFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void SetsValuesCorrectly()
            {
                var exception = new PropertyNotNullableException("PropertyName", typeof (string));
                Assert.AreEqual("PropertyName", exception.PropertyName);
                Assert.AreEqual(typeof (string), exception.PropertyType);
            }
        }
    }
}