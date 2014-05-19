// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularDependencyExceptionFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.IoC.Exceptions
{
    using System;
    using Catel.IoC;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class CircularDependencyExceptionFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullTypePath()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new CircularDependencyException(null, string.Empty));
            }
        }
    }
}