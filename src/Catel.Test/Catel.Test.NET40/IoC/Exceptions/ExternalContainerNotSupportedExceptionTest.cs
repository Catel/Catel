// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalContainerNotSupportedExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.IoC.Exceptions
{
    using Catel.IoC;
    
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class ExternalContainerNotSupportedExceptionTest
    {
        #region Methods
        [TestMethod]
        public void FormatMessage_SupportedContainers()
        {
            string expectedString = @"The specified container is not supported. Please use one of the following:
  * Unity
  * Another IoC
";
            var ex = new ExternalContainerNotSupportedException(new[] {"Unity", "Another IoC"});

            Assert.AreEqual(expectedString, ex.Message);
        }
        #endregion
    }
}