// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidPropertyExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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

    public class InvalidPropertyExceptionFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void SetsValuesCorrectly()
            {
                var exception = new InvalidPropertyException("PropertyName");
                Assert.AreEqual("PropertyName", exception.PropertyName);
            }
        }
    }
}