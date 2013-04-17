// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationSummaryTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System;
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ValidationSummaryFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidationContextWithoutTag()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ValidationSummary(null));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidationContextWithTag()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ValidationSummary(null, "tag"));
            }

            [TestMethod]
            public void ReturnsCorrectSummaryWithoutTag()
            {
                var context = ValidationContextFacts.CreateValidationContext();

                var summary = new ValidationSummary(context);

                Assert.AreEqual(2, summary.FieldWarnings.Count);
                Assert.AreEqual(2, summary.FieldErrors.Count);
                Assert.AreEqual(2, summary.BusinessWarnings.Count);
                Assert.AreEqual(2, summary.BusinessRuleErrors.Count);
            }

            [TestMethod]
            public void ReturnsCorrectSummaryWithTag()
            {
                var context = ValidationContextFacts.CreateValidationContext();

                var summary = new ValidationSummary(context, "tag");

                Assert.AreEqual(1, summary.FieldWarnings.Count);
                Assert.AreEqual(1, summary.FieldErrors.Count);
                Assert.AreEqual(1, summary.BusinessWarnings.Count);
                Assert.AreEqual(1, summary.BusinessRuleErrors.Count);
            }
        }
    }
}