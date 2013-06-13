// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayNameAttributeTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.FluentValidation.Attributes
{
    using System;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class DisplayNameAttributeFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullDisplayName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new DisplayNameAttribute(null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyDisplayName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new DisplayNameAttribute(string.Empty));
            }

            [TestMethod]
            public void CorrectlySetsValues()
            {
                var displayNameAttribute = new DisplayNameAttribute("FirstName");
                Assert.AreEqual("FirstName", displayNameAttribute.DisplayName);

                displayNameAttribute = new DisplayNameAttribute("First Name");
                Assert.AreEqual("First Name", displayNameAttribute.DisplayName);
            }
        }
    }
}