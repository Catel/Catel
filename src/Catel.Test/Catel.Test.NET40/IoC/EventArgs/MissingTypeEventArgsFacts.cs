// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MissingTypeEventArgsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.IoC.EventArgs
{
    using System;
    using Catel.IoC;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class MissingTypeEventArgsFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInterfaceType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new MissingTypeEventArgs(null));
            }

            [TestMethod]
            public void SetsValuesCorrectly()
            {
                var eventArgs = new MissingTypeEventArgs(typeof (ITestInterface));

                Assert.AreEqual(typeof (ITestInterface), eventArgs.InterfaceType);
            }
        }
    }
}