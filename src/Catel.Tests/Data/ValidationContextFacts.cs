namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

    using NUnit.Framework;

    public class ValidationContextFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void AcceptsNullArguments()
            {
                var validationContext = new ValidationContext(null, null);

                Assert.That(validationContext.GetFieldValidationCount(), Is.EqualTo(0));
                Assert.That(validationContext.GetBusinessRuleValidationCount(), Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class TheHasWarningsProperty
        {
            [TestCase]
            public void ReturnsFalseForValidationContextWithoutWarnings()
            {
                var context = new ValidationContext();

                Assert.That(context.HasWarnings, Is.False);
            }

            [TestCase]
            public void ReturnsTrueForValidationContextWithFieldWarnings()
            {
                var context = CreateValidationContextSimple(true, false, false, false);

                Assert.That(context.HasWarnings, Is.True);
            }

            [TestCase]
            public void ReturnsTrueForValidationContextWithBusinessRuleWarnings()
            {
                var context = CreateValidationContextSimple(false, false, true, false);

                Assert.That(context.HasWarnings, Is.True);
            }
        }

        [TestFixture]
        public class TheHasErrorsProperty
        {
            [TestCase]
            public void ReturnsFalseForValidationContextWithoutErrors()
            {
                var context = new ValidationContext();

                Assert.That(context.HasErrors, Is.False);
            }

            [TestCase]
            public void ReturnsTrueForValidationContextWithFieldErrors()
            {
                var context = CreateValidationContextSimple(false, true, false, false);

                Assert.That(context.HasErrors, Is.True);
            }

            [TestCase]
            public void ReturnsTrueForValidationContextWithBusinessRuleErrors()
            {
                var context = CreateValidationContextSimple(false, false, false, true);

                Assert.That(context.HasErrors, Is.True);
            }
        }

        [TestFixture]
        public class TheGetValidationCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.That(context.GetValidationCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetValidationCount(), Is.EqualTo(8));
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.That(context.GetValidationCount("tag"), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetValidationCount("tag"), Is.EqualTo(4));
            }
        }

        [TestFixture]
        public class TheGetValidationsMethods
        {
            [TestCase]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var validations = context.GetValidations();
                Assert.That(validations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var validations = context.GetValidations();
                Assert.That(validations.Count, Is.EqualTo(8));
                Assert.That(validations[0].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(validations[0] is IFieldValidationResult, Is.True);
                Assert.That(validations[1].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(validations[1] is IFieldValidationResult, Is.True);
                Assert.That(validations[2].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(validations[2] is IFieldValidationResult, Is.True);
                Assert.That(validations[3].Message, Is.EqualTo("ErrorWithTag"));
                Assert.That(validations[3] is IFieldValidationResult, Is.True);
                Assert.That(validations[4].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(validations[4] is IBusinessRuleValidationResult, Is.True);
                Assert.That(validations[5].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(validations[5] is IBusinessRuleValidationResult, Is.True);
                Assert.That(validations[6].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(validations[6] is IBusinessRuleValidationResult, Is.True);
                Assert.That(validations[7].Message, Is.EqualTo("ErrorWithTag"));
                Assert.That(validations[7] is IBusinessRuleValidationResult, Is.True);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var validations = context.GetValidations("tag");
                Assert.That(validations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var validations = context.GetValidations("tag");
                Assert.That(validations.Count, Is.EqualTo(4));
                Assert.That(validations[0].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(validations[0] is IFieldValidationResult, Is.True);
                Assert.That(validations[1].Message, Is.EqualTo("ErrorWithTag"));
                Assert.That(validations[1] is IFieldValidationResult, Is.True);
                Assert.That(validations[2].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(validations[2] is IBusinessRuleValidationResult, Is.True);
                Assert.That(validations[3].Message, Is.EqualTo("ErrorWithTag"));
                Assert.That(validations[3] is IBusinessRuleValidationResult, Is.True);
            }
        }

        [TestFixture]
        public class TheGetWarningCount
        {
            [TestCase]
            public void ReturnsZeroForContextWithoutWarnings()
            {
                var context = new ValidationContext();

                Assert.That(context.GetWarningCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetWarningCount(), Is.EqualTo(4));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetWarningCount("tag"), Is.EqualTo(2));
                Assert.That(context.GetWarningCount(null), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheGetWarningsMethod
        {
            [TestCase]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings();
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var warnings = context.GetWarnings();
                Assert.That(warnings.Count, Is.EqualTo(4));

                Assert.That(warnings[0].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(warnings[0] is IFieldValidationResult, Is.True);

                Assert.That(warnings[1].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(warnings[1] is IFieldValidationResult, Is.True);

                Assert.That(warnings[2].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(warnings[2] is IBusinessRuleValidationResult, Is.True);

                Assert.That(warnings[3].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(warnings[3] is IBusinessRuleValidationResult, Is.True);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var warnings = context.GetWarnings("tag");
                Assert.That(warnings.Count, Is.EqualTo(2));

                Assert.That(warnings[0].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(warnings[0] is IFieldValidationResult, Is.True);

                Assert.That(warnings[1].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(warnings[1] is IBusinessRuleValidationResult, Is.True);
            }
        }

        [TestFixture]
        public class TheGetErrorCount
        {
            [TestCase]
            public void ReturnsZeroForContextWithoutErrors()
            {
                var context = new ValidationContext();

                Assert.That(context.GetWarningCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetErrorCount(), Is.EqualTo(4));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetErrorCount("tag"), Is.EqualTo(2));
                Assert.That(context.GetErrorCount(null), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheGetErrorsMethod
        {
            [TestCase]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors();
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var errors = context.GetErrors();
                Assert.That(errors.Count, Is.EqualTo(4));

                Assert.That(errors[0].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(errors[0] is IFieldValidationResult, Is.True);

                Assert.That(errors[1].Message, Is.EqualTo("ErrorWithTag"));
                Assert.That(errors[1] is IFieldValidationResult, Is.True);

                Assert.That(errors[2].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(errors[2] is IBusinessRuleValidationResult, Is.True);

                Assert.That(errors[3].Message, Is.EqualTo("ErrorWithTag"));
                Assert.That(errors[3] is IBusinessRuleValidationResult, Is.True);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var warnings = context.GetErrors("tag");
                Assert.That(warnings.Count, Is.EqualTo(2));

                Assert.That(warnings[0].Message, Is.EqualTo("ErrorWithTag"));
                Assert.That(warnings[0] is IFieldValidationResult, Is.True);

                Assert.That(warnings[1].Message, Is.EqualTo("ErrorWithTag"));
                Assert.That(warnings[1] is IBusinessRuleValidationResult, Is.True);
            }
        }

        [TestFixture]
        public class TheGetFieldValidationCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.That(context.GetFieldValidationCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetFieldValidationCount(), Is.EqualTo(4));
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.That(context.GetFieldValidationCount("tag"), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetFieldValidationCount("tag"), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheGetFieldValidationsMethod
        {
            [TestCase]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(4));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(fieldValidations[1].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(fieldValidations[2].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(fieldValidations[3].Message, Is.EqualTo("ErrorWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations((object)"tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations((object)"tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(fieldValidations[1].Message, Is.EqualTo("ErrorWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty");
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty");
                Assert.That(fieldValidations.Count, Is.EqualTo(4));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(fieldValidations[1].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(fieldValidations[2].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(fieldValidations[3].Message, Is.EqualTo("ErrorWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTagAndPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty", "tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTagAndPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty", "tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(fieldValidations[1].Message, Is.EqualTo("ErrorWithTag"));
            }
        }

        [TestFixture]
        public class TheGetFieldWarningCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.That(context.GetFieldWarningCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetFieldWarningCount(), Is.EqualTo(2));
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.That(context.GetFieldWarningCount("tag"), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetFieldWarningCount("tag"), Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class TheGetFieldWarningsMethod
        {
            [TestCase]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings();
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings();
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(fieldValidations[1].Message, Is.EqualTo("WarningWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings((object)"tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings((object)"tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(1));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("WarningWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty");
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty");
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(fieldValidations[1].Message, Is.EqualTo("WarningWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTagAndPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty", "tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTagAndPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty", "tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(1));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("WarningWithTag"));
            }
        }

        [TestFixture]
        public class TheGetFieldErrorCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.That(context.GetFieldErrorCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetFieldErrorCount(), Is.EqualTo(2));
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.That(context.GetFieldErrorCount("tag"), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetFieldErrorCount("tag"), Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class TheGetFieldErrorsMethod
        {
            [TestCase]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors();
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors();
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(fieldValidations[1].Message, Is.EqualTo("ErrorWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors("tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors((object)"tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(1));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("ErrorWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty");
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty");
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(fieldValidations[1].Message, Is.EqualTo("ErrorWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTagAndPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty");
                Assert.That(fieldValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTagAndPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty", "tag");
                Assert.That(fieldValidations.Count, Is.EqualTo(1));
                Assert.That(fieldValidations[0].Message, Is.EqualTo("ErrorWithTag"));
            }
        }

        [TestFixture]
        public class TheGetBusinessRuleValidationCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.That(context.GetBusinessRuleValidationCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetBusinessRuleValidationCount(), Is.EqualTo(4));
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.That(context.GetBusinessRuleValidationCount("tag"), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetBusinessRuleValidationCount("tag"), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheGetBusinessRuleValidationsMethod
        {
            [TestCase]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(4));
                Assert.That(businessRuleValidations[0].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(businessRuleValidations[1].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(businessRuleValidations[2].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(businessRuleValidations[3].Message, Is.EqualTo("ErrorWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations("tag");
                Assert.That(businessRuleValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations("tag");
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                Assert.That(businessRuleValidations[0].Message, Is.EqualTo("WarningWithTag"));
                Assert.That(businessRuleValidations[1].Message, Is.EqualTo("ErrorWithTag"));
            }
        }

        [TestFixture]
        public class TheGetBusinessRuleWarningCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.That(context.GetBusinessRuleWarningCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetBusinessRuleWarningCount(), Is.EqualTo(2));
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.That(context.GetBusinessRuleWarningCount("tag"), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetBusinessRuleWarningCount("tag"), Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class TheGetBusinessRuleWarningsMethod
        {
            [TestCase]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                Assert.That(businessRuleValidations[0].Message, Is.EqualTo("WarningWithoutTag"));
                Assert.That(businessRuleValidations[1].Message, Is.EqualTo("WarningWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings("tag");
                Assert.That(businessRuleValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings("tag");
                Assert.That(businessRuleValidations.Count, Is.EqualTo(1));
                Assert.That(businessRuleValidations[0].Message, Is.EqualTo("WarningWithTag"));
            }
        }

        [TestFixture]
        public class TheGetBusinessRuleErrorCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.That(context.GetBusinessRuleErrorCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetBusinessRuleErrorCount(), Is.EqualTo(2));
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.That(context.GetBusinessRuleErrorCount("tag"), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.That(context.GetBusinessRuleErrorCount("tag"), Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class TheGetBusinessRuleErrorsMethod
        {
            [TestCase]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                Assert.That(businessRuleValidations[0].Message, Is.EqualTo("ErrorWithoutTag"));
                Assert.That(businessRuleValidations[1].Message, Is.EqualTo("ErrorWithTag"));
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors("tag");
                Assert.That(businessRuleValidations.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors("tag");
                Assert.That(businessRuleValidations.Count, Is.EqualTo(1));
                Assert.That(businessRuleValidations[0].Message, Is.EqualTo("ErrorWithTag"));
            }
        }

        [TestFixture]
        public class TheAddFieldValidationResultMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                Assert.Throws<ArgumentNullException>(() => validationContext.Add((IFieldValidationResult)null));
            }

            [TestCase]
            public void AddsSingleValidationResultForField()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                Assert.That(validationContext.GetFieldValidationCount(), Is.EqualTo(0));

                validationContext.Add(fieldValidation);

                Assert.That(validationContext.GetFieldValidationCount(), Is.EqualTo(1));
            }

            [TestCase]
            public void AddsMultipleValidationResultsForField()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                validationContext.Add(fieldValidation);

                Assert.That(validationContext.GetFieldValidationCount(), Is.EqualTo(1));

                validationContext.Add(fieldValidation);

                Assert.That(validationContext.GetFieldValidationCount(), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheRemoveFieldValidationResultMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                Assert.Throws<ArgumentNullException>(() => validationContext.Remove((IFieldValidationResult)null));
            }

            [TestCase]
            public void CorrectlyHandlesRemovalOfExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                validationContext.Add(fieldValidation);

                Assert.That(validationContext.GetFieldValidationCount(), Is.EqualTo(1));

                validationContext.Remove(fieldValidation);

                Assert.That(validationContext.GetFieldValidationCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void CorrectlyHandlesRemovalOfNonExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                Assert.That(validationContext.GetFieldValidationCount(), Is.EqualTo(0));

                validationContext.Remove(fieldValidation);

                Assert.That(validationContext.GetFieldValidationCount(), Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class TheAddBusinessRuleValidationResultMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                Assert.Throws<ArgumentNullException>(() => validationContext.Add((IBusinessRuleValidationResult)null));
            }

            [TestCase]
            public void AddsValidationResultOnce()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyProperty");

                Assert.That(validationContext.GetBusinessRuleValidationCount(), Is.EqualTo(0));

                validationContext.Add(businessRuleValidation);

                Assert.That(validationContext.GetBusinessRuleValidationCount(), Is.EqualTo(1));
            }

            [TestCase]
            public void AddsValidationResultTwice()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyProperty");

                validationContext.Add(businessRuleValidation);

                Assert.That(validationContext.GetBusinessRuleValidationCount(), Is.EqualTo(1));

                validationContext.Add(businessRuleValidation);

                Assert.That(validationContext.GetBusinessRuleValidationCount(), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheRemoveBusinessRuleValidationMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                Assert.Throws<ArgumentNullException>(() => validationContext.Remove((IBusinessRuleValidationResult)null));
            }

            [TestCase]
            public void CorrectlyHandlesRemovalOfExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyError");

                validationContext.Add(businessRuleValidation);

                Assert.That(validationContext.GetBusinessRuleValidationCount(), Is.EqualTo(1));

                validationContext.Remove(businessRuleValidation);

                Assert.That(validationContext.GetBusinessRuleValidationCount(), Is.EqualTo(0));
            }

            [TestCase]
            public void CorrectlyHandlesRemovalOfNonExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyError");

                Assert.That(validationContext.GetBusinessRuleValidationCount(), Is.EqualTo(0));

                validationContext.Remove(businessRuleValidation);

                Assert.That(validationContext.GetBusinessRuleValidationCount(), Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class TheSynchronizeWithContextMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationContext()
            {
                Assert.Throws<ArgumentNullException>(() => new ValidationContext().SynchronizeWithContext(null));
            }

            [TestCase]
            public void CorrectlySynchronizesWithNoChanges()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                context1.SynchronizeWithContext(context2);

                // First context should now equal second context
                int counter;

                var fieldValidations = context1.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldWarning"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldError"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));
            }

            [TestCase]
            public void CorrectlySynchronizesWithAddedFieldWarning()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                context1.SynchronizeWithContext(context2);

                // First context should now equal second context
                int counter;

                var fieldValidations = context1.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldError"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldWarning"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));
            }

            [TestCase]
            public void CorrectlySynchronizesWithRemovedFieldWarning()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                context1.SynchronizeWithContext(context2);

                // First context should now equal second context
                int counter;

                var fieldValidations = context1.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(1));
                counter = -1;

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldError"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));
            }

            [TestCase]
            public void CorrectlySynchronizesWithAddedFieldError()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                context1.SynchronizeWithContext(context2);

                // First context should now equal second context
                int counter;

                var fieldValidations = context1.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldWarning"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldError"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));
            }

            [TestCase]
            public void CorrectlySynchronizesWithRemovedFieldError()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                context1.SynchronizeWithContext(context2);

                // First context should now equal second context
                int counter;

                var fieldValidations = context1.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(1));
                counter = -1;

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldWarning"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));
            }

            [TestCase]
            public void CorrectlySynchronizesWithAddedBusinessRuleWarning()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                context1.SynchronizeWithContext(context2);

                // First context should now equal second context
                int counter;

                var fieldValidations = context1.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldWarning"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldError"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
            }

            [TestCase]
            public void CorrectlySynchronizesWithRemovedBusinessRuleWarning()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                context1.SynchronizeWithContext(context2);

                // First context should now equal second context
                int counter;

                var fieldValidations = context1.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldWarning"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldError"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(1));
                counter = -1;

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));
            }

            [TestCase]
            public void CorrectlySynchronizesWithAddedBusinessRuleError()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                context1.SynchronizeWithContext(context2);

                // First context should now equal second context
                int counter;

                var fieldValidations = context1.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldWarning"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldError"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));
            }

            [TestCase]
            public void CorrectlySynchronizesWithRemovedBusinessRuleError()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                context1.SynchronizeWithContext(context2);

                // First context should now equal second context
                int counter;

                var fieldValidations = context1.GetFieldValidations();
                Assert.That(fieldValidations.Count, Is.EqualTo(2));
                counter = -1;

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldWarning"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));

                counter++;
                Assert.That(fieldValidations[counter].PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(fieldValidations[counter].Message, Is.EqualTo("FieldError"));
                Assert.That(fieldValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Error));

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.That(businessRuleValidations.Count, Is.EqualTo(1));
                counter = -1;

                counter++;
                Assert.That(businessRuleValidations[counter].Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(businessRuleValidations[counter].ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
            }
        }

        public static ValidationContext CreateValidationContextSimple(bool createFieldWarning = true, bool createFieldError = true,
            bool createBusinessRuleWarning = true, bool createBusinessRuleError = true)
        {
            return CreateValidationContext(createFieldWarning, createFieldWarning, createFieldError, createFieldError,
                createBusinessRuleWarning, createBusinessRuleWarning, createBusinessRuleError, createBusinessRuleError);
        }

        public static ValidationContext CreateValidationContext(bool createFieldWarning = true, bool createFieldWarningWithTag = true,
            bool createFieldError = true, bool createFieldErrorWithTag = true, bool createBusinessRuleWarning = true,
            bool createBusinessRuleWarningWithTag = true, bool createBusinessRuleError = true, bool createBusinessRuleErrorWithTag = true)
        {
            var fieldValidations = new List<IFieldValidationResult>();

            if (createFieldWarning)
            {
                fieldValidations.Add(FieldValidationResult.CreateWarning("MyProperty", "WarningWithoutTag"));
            }

            if (createFieldWarningWithTag)
            {
                fieldValidations.Add(new FieldValidationResult("MyProperty", ValidationResultType.Warning, "WarningWithTag") { Tag = "tag" });
            }

            if (createFieldError)
            {
                fieldValidations.Add(FieldValidationResult.CreateError("MyProperty", "ErrorWithoutTag"));
            }

            if (createFieldErrorWithTag)
            {
                fieldValidations.Add(new FieldValidationResult("MyProperty", ValidationResultType.Error, "ErrorWithTag") { Tag = "tag" });
            }

            var businessRuleValidations = new List<IBusinessRuleValidationResult>();

            if (createBusinessRuleWarning)
            {
                businessRuleValidations.Add(new BusinessRuleValidationResult(ValidationResultType.Warning, "WarningWithoutTag"));
            }

            if (createBusinessRuleWarningWithTag)
            {
                businessRuleValidations.Add(new BusinessRuleValidationResult(ValidationResultType.Warning, "WarningWithTag") { Tag = "tag" });
            }

            if (createBusinessRuleError)
            {
                businessRuleValidations.Add(new BusinessRuleValidationResult(ValidationResultType.Error, "ErrorWithoutTag"));
            }

            if (createBusinessRuleErrorWithTag)
            {
                businessRuleValidations.Add(new BusinessRuleValidationResult(ValidationResultType.Error, "ErrorWithTag") { Tag = "tag" });
            }

            return new ValidationContext(fieldValidations, businessRuleValidations);
        }
    }
}