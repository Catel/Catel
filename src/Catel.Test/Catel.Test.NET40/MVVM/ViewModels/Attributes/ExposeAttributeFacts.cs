// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExposeAttributeFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.Attributes
{
    using System;
    using Catel.MVVM;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ExposeAttributeFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new ExposeAttribute(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new ExposeAttribute(string.Empty));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new ExposeAttribute(" "));
            }

            [TestMethod]
            public void CorrectlySetsProperties()
            {
                var exposeAttribute = new ExposeAttribute("myProperty");

                Assert.AreEqual("myProperty", exposeAttribute.PropertyName);
                Assert.AreEqual("myProperty", exposeAttribute.PropertyNameOnModel);
                Assert.AreEqual(ViewModelToModelMode.TwoWay, exposeAttribute.Mode);
            }
        }
    }
}