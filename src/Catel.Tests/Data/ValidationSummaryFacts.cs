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

                Assert.That(summary.FieldWarnings.Count, Is.EqualTo(2));
                Assert.That(summary.FieldErrors.Count, Is.EqualTo(2));
                Assert.That(summary.BusinessRuleWarnings.Count, Is.EqualTo(2));
                Assert.That(summary.BusinessRuleErrors.Count, Is.EqualTo(2));
            }

            [TestCase]
            public void ReturnsCorrectSummaryWithTag()
            {
                var context = ValidationContextFacts.CreateValidationContext();

                var summary = new ValidationSummary(context, "tag");

                Assert.That(summary.FieldWarnings.Count, Is.EqualTo(1));
                Assert.That(summary.FieldErrors.Count, Is.EqualTo(1));
                Assert.That(summary.BusinessRuleWarnings.Count, Is.EqualTo(1));
                Assert.That(summary.BusinessRuleErrors.Count, Is.EqualTo(1));
            }
        }
    }
}
