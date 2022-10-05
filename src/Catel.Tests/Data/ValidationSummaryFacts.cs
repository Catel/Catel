namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;

    using NUnit.Framework;

    public class ValidationSummaryFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationContextWithoutTag()
            {
                Assert.Throws<ArgumentNullException>(() => new ValidationSummary(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationContextWithTag()
            {
                Assert.Throws<ArgumentNullException>(() => new ValidationSummary(null, "tag"));
            }

            [TestCase]
            public void ReturnsCorrectSummaryWithoutTag()
            {
                var context = ValidationContextFacts.CreateValidationContext();

                var summary = new ValidationSummary(context);

                Assert.AreEqual(2, summary.FieldWarnings.Count);
                Assert.AreEqual(2, summary.FieldErrors.Count);
                Assert.AreEqual(2, summary.BusinessRuleWarnings.Count);
                Assert.AreEqual(2, summary.BusinessRuleErrors.Count);
            }

            [TestCase]
            public void ReturnsCorrectSummaryWithTag()
            {
                var context = ValidationContextFacts.CreateValidationContext();

                var summary = new ValidationSummary(context, "tag");

                Assert.AreEqual(1, summary.FieldWarnings.Count);
                Assert.AreEqual(1, summary.FieldErrors.Count);
                Assert.AreEqual(1, summary.BusinessRuleWarnings.Count);
                Assert.AreEqual(1, summary.BusinessRuleErrors.Count);
            }
        }
    }
}
