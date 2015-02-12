// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayNameAttributeTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Test.Extensions.FluentValidation.Attributes
{
    using System;

    using NUnit.Framework;

    public class DisplayNameAttributeFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullDisplayName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new DisplayNameAttribute(null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyDisplayName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new DisplayNameAttribute(string.Empty));
            }

            [TestCase]
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

#endif