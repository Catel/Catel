// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowNotRegisteredExceptionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Services.Exceptions
{
    using Catel.MVVM.Services;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class WindowNotRegisteredExceptionTest
    {
        #region Methods
        [TestMethod]
        public void Constructor()
        {
            try
            {
                throw new WindowNotRegisteredException("windowName");
            }
            catch (WindowNotRegisteredException ex)
            {
                Assert.AreEqual("windowName", ex.Name);
            }
        }
        #endregion
    }
}