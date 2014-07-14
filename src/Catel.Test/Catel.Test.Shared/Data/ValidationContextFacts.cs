// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationContextTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
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

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());
                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());
            }
        }

        [TestFixture]
        public class TheHasWarningsProperty
        {
            [TestCase]
            public void ReturnsFalseForValidationContextWithoutWarnings()
            {
                var context = new ValidationContext();

                Assert.IsFalse(context.HasWarnings);
            }

            [TestCase]
            public void ReturnsTrueForValidationContextWithFieldWarnings()
            {
                var context = CreateValidationContextSimple(true, false, false, false);

                Assert.IsTrue(context.HasWarnings);
            }

            [TestCase]
            public void ReturnsTrueForValidationContextWithBusinessRuleWarnings()
            {
                var context = CreateValidationContextSimple(false, false, true, false);

                Assert.IsTrue(context.HasWarnings);
            }
        }

        [TestFixture]
        public class TheHasErrorsProperty
        {
            [TestCase]
            public void ReturnsFalseForValidationContextWithoutErrors()
            {
                var context = new ValidationContext();

                Assert.IsFalse(context.HasErrors);
            }

            [TestCase]
            public void ReturnsTrueForValidationContextWithFieldErrors()
            {
                var context = CreateValidationContextSimple(false, true, false, false);

                Assert.IsTrue(context.HasErrors);
            }

            [TestCase]
            public void ReturnsTrueForValidationContextWithBusinessRuleErrors()
            {
                var context = CreateValidationContextSimple(false, false, false, true);

                Assert.IsTrue(context.HasErrors);
            }
        }

        [TestFixture]
        public class TheGetValidationCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetValidationCount());
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(8, context.GetValidationCount());
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetValidationCount("tag"));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetValidationCount("tag"));
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
                Assert.AreEqual(0, validations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var validations = context.GetValidations();
                Assert.AreEqual(8, validations.Count);
                Assert.AreEqual("WarningWithoutTag", validations[0].Message);
                Assert.IsTrue(validations[0] is IFieldValidationResult);
                Assert.AreEqual("WarningWithTag", validations[1].Message);
                Assert.IsTrue(validations[1] is IFieldValidationResult);
                Assert.AreEqual("ErrorWithoutTag", validations[2].Message);
                Assert.IsTrue(validations[2] is IFieldValidationResult);
                Assert.AreEqual("ErrorWithTag", validations[3].Message);
                Assert.IsTrue(validations[3] is IFieldValidationResult);
                Assert.AreEqual("WarningWithoutTag", validations[4].Message);
                Assert.IsTrue(validations[4] is IBusinessRuleValidationResult);
                Assert.AreEqual("WarningWithTag", validations[5].Message);
                Assert.IsTrue(validations[5] is IBusinessRuleValidationResult);
                Assert.AreEqual("ErrorWithoutTag", validations[6].Message);
                Assert.IsTrue(validations[6] is IBusinessRuleValidationResult);
                Assert.AreEqual("ErrorWithTag", validations[7].Message);
                Assert.IsTrue(validations[7] is IBusinessRuleValidationResult);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var validations = context.GetValidations("tag");
                Assert.AreEqual(0, validations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var validations = context.GetValidations("tag");
                Assert.AreEqual(4, validations.Count);
                Assert.AreEqual("WarningWithTag", validations[0].Message);
                Assert.IsTrue(validations[0] is IFieldValidationResult);
                Assert.AreEqual("ErrorWithTag", validations[1].Message);
                Assert.IsTrue(validations[1] is IFieldValidationResult);
                Assert.AreEqual("WarningWithTag", validations[2].Message);
                Assert.IsTrue(validations[2] is IBusinessRuleValidationResult);
                Assert.AreEqual("ErrorWithTag", validations[3].Message);
                Assert.IsTrue(validations[3] is IBusinessRuleValidationResult);
            }
        }

        [TestFixture]
        public class TheGetWarningCount
        {
            [TestCase]
            public void ReturnsZeroForContextWithoutWarnings()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetWarningCount());
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetWarningCount());
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetWarningCount("tag"));
                Assert.AreEqual(2, context.GetWarningCount(null));
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
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var warnings = context.GetWarnings();
                Assert.AreEqual(4, warnings.Count);

                Assert.AreEqual("WarningWithoutTag", warnings[0].Message);
                Assert.IsTrue(warnings[0] is IFieldValidationResult);

                Assert.AreEqual("WarningWithTag", warnings[1].Message);
                Assert.IsTrue(warnings[1] is IFieldValidationResult);

                Assert.AreEqual("WarningWithoutTag", warnings[2].Message);
                Assert.IsTrue(warnings[2] is IBusinessRuleValidationResult);

                Assert.AreEqual("WarningWithTag", warnings[3].Message);
                Assert.IsTrue(warnings[3] is IBusinessRuleValidationResult);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var warnings = context.GetWarnings("tag");
                Assert.AreEqual(2, warnings.Count);

                Assert.AreEqual("WarningWithTag", warnings[0].Message);
                Assert.IsTrue(warnings[0] is IFieldValidationResult);

                Assert.AreEqual("WarningWithTag", warnings[1].Message);
                Assert.IsTrue(warnings[1] is IBusinessRuleValidationResult);
            }
        }

        [TestFixture]
        public class TheGetErrorCount
        {
            [TestCase]
            public void ReturnsZeroForContextWithoutErrors()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetWarningCount());
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetErrorCount());
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetErrorCount("tag"));
                Assert.AreEqual(2, context.GetErrorCount(null));
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
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var errors = context.GetErrors();
                Assert.AreEqual(4, errors.Count);

                Assert.AreEqual("ErrorWithoutTag", errors[0].Message);
                Assert.IsTrue(errors[0] is IFieldValidationResult);

                Assert.AreEqual("ErrorWithTag", errors[1].Message);
                Assert.IsTrue(errors[1] is IFieldValidationResult);

                Assert.AreEqual("ErrorWithoutTag", errors[2].Message);
                Assert.IsTrue(errors[2] is IBusinessRuleValidationResult);

                Assert.AreEqual("ErrorWithTag", errors[3].Message);
                Assert.IsTrue(errors[3] is IBusinessRuleValidationResult);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var warnings = context.GetErrors("tag");
                Assert.AreEqual(2, warnings.Count);

                Assert.AreEqual("ErrorWithTag", warnings[0].Message);
                Assert.IsTrue(warnings[0] is IFieldValidationResult);

                Assert.AreEqual("ErrorWithTag", warnings[1].Message);
                Assert.IsTrue(warnings[1] is IBusinessRuleValidationResult);
            }
        }

        [TestFixture]
        public class TheGetFieldValidationCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldValidationCount());
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetFieldValidationCount());
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldValidationCount("tag"));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetFieldValidationCount("tag"));
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
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations();
                Assert.AreEqual(4, fieldValidations.Count);
                Assert.AreEqual("WarningWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("WarningWithTag", fieldValidations[1].Message);
                Assert.AreEqual("ErrorWithoutTag", fieldValidations[2].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[3].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations((object)"tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations((object)"tag");
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("WarningWithTag", fieldValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[1].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty");
                Assert.AreEqual(4, fieldValidations.Count);
                Assert.AreEqual("WarningWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("WarningWithTag", fieldValidations[1].Message);
                Assert.AreEqual("ErrorWithoutTag", fieldValidations[2].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[3].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTagAndPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty", "tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTagAndPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty", "tag");
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("WarningWithTag", fieldValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[1].Message);
            }
        }

        [TestFixture]
        public class TheGetFieldWarningCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldWarningCount());
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetFieldWarningCount());
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldWarningCount("tag"));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(1, context.GetFieldWarningCount("tag"));
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
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings();
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("WarningWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("WarningWithTag", fieldValidations[1].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings((object)"tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings((object)"tag");
                Assert.AreEqual(1, fieldValidations.Count);
                Assert.AreEqual("WarningWithTag", fieldValidations[0].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty");
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("WarningWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("WarningWithTag", fieldValidations[1].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTagAndPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty", "tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTagAndPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty", "tag");
                Assert.AreEqual(1, fieldValidations.Count);
                Assert.AreEqual("WarningWithTag", fieldValidations[0].Message);
            }
        }

        [TestFixture]
        public class TheGetFieldErrorCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldErrorCount());
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetFieldErrorCount());
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldErrorCount("tag"));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(1, context.GetFieldErrorCount("tag"));
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
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors();
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("ErrorWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[1].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors("tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors((object)"tag");
                Assert.AreEqual(1, fieldValidations.Count);
                Assert.AreEqual("ErrorWithTag", fieldValidations[0].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty");
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("ErrorWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[1].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTagAndPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTagAndPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty", "tag");
                Assert.AreEqual(1, fieldValidations.Count);
                Assert.AreEqual("ErrorWithTag", fieldValidations[0].Message);
            }
        }

        [TestFixture]
        public class TheGetBusinessRuleValidationCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleValidationCount());
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetBusinessRuleValidationCount());
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleValidationCount("tag"));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetBusinessRuleValidationCount("tag"));
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
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations();
                Assert.AreEqual(4, businessRuleValidations.Count);
                Assert.AreEqual("WarningWithoutTag", businessRuleValidations[0].Message);
                Assert.AreEqual("WarningWithTag", businessRuleValidations[1].Message);
                Assert.AreEqual("ErrorWithoutTag", businessRuleValidations[2].Message);
                Assert.AreEqual("ErrorWithTag", businessRuleValidations[3].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations("tag");
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations("tag");
                Assert.AreEqual(2, businessRuleValidations.Count);
                Assert.AreEqual("WarningWithTag", businessRuleValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", businessRuleValidations[1].Message);
            }
        }

        [TestFixture]
        public class TheGetBusinessRuleWarningCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleWarningCount());
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetBusinessRuleWarningCount());
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleWarningCount("tag"));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(1, context.GetBusinessRuleWarningCount("tag"));
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
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings();
                Assert.AreEqual(2, businessRuleValidations.Count);
                Assert.AreEqual("WarningWithoutTag", businessRuleValidations[0].Message);
                Assert.AreEqual("WarningWithTag", businessRuleValidations[1].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings("tag");
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings("tag");
                Assert.AreEqual(1, businessRuleValidations.Count);
                Assert.AreEqual("WarningWithTag", businessRuleValidations[0].Message);
            }
        }

        [TestFixture]
        public class TheGetBusinessRuleErrorCountMethod
        {
            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleErrorCount());
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetBusinessRuleErrorCount());
            }

            [TestCase]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleErrorCount("tag"));
            }

            [TestCase]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(1, context.GetBusinessRuleErrorCount("tag"));
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
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors();
                Assert.AreEqual(2, businessRuleValidations.Count);
                Assert.AreEqual("ErrorWithoutTag", businessRuleValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", businessRuleValidations[1].Message);
            }

            [TestCase]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors("tag");
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestCase]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors("tag");
                Assert.AreEqual(1, businessRuleValidations.Count);
                Assert.AreEqual("ErrorWithTag", businessRuleValidations[0].Message);
            }
        }

        [TestFixture]
        public class TheAddFieldValidationResultMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => validationContext.AddFieldValidationResult(null));
            }

            [TestCase]
            public void AddsSingleValidationResultForField()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());

                validationContext.AddFieldValidationResult(fieldValidation);

                Assert.AreEqual(1, validationContext.GetFieldValidationCount());
            }

            [TestCase]
            public void AddsMultipleValidationResultsForField()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                validationContext.AddFieldValidationResult(fieldValidation);

                Assert.AreEqual(1, validationContext.GetFieldValidationCount());

                validationContext.AddFieldValidationResult(fieldValidation);

                Assert.AreEqual(2, validationContext.GetFieldValidationCount());
            }
        }

        [TestFixture]
        public class TheRemoveFieldValidationResultMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => validationContext.RemoveFieldValidationResult(null));
            }

            [TestCase]
            public void CorrectlyHandlesRemovalOfExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                validationContext.AddFieldValidationResult(fieldValidation);

                Assert.AreEqual(1, validationContext.GetFieldValidationCount());

                validationContext.RemoveFieldValidationResult(fieldValidation);

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());
            }

            [TestCase]
            public void CorrectlyHandlesRemovalOfNonExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());

                validationContext.RemoveFieldValidationResult(fieldValidation);

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());
            }
        }

        [TestFixture]
        public class TheAddBusinessRuleValidationResultMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => validationContext.AddBusinessRuleValidationResult(null));
            }

            [TestCase]
            public void AddsValidationResultOnce()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyProperty");

                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());

                validationContext.AddBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(1, validationContext.GetBusinessRuleValidationCount());
            }

            [TestCase]
            public void AddsValidationResultTwice()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyProperty");

                validationContext.AddBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(1, validationContext.GetBusinessRuleValidationCount());

                validationContext.AddBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(2, validationContext.GetBusinessRuleValidationCount());
            }
        }

        [TestFixture]
        public class TheRemoveBusinessRuleValidationMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => validationContext.RemoveBusinessRuleValidationResult(null));
            }

            [TestCase]
            public void CorrectlyHandlesRemovalOfExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyError");

                validationContext.AddBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(1, validationContext.GetBusinessRuleValidationCount());

                validationContext.RemoveBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());
            }

            [TestCase]
            public void CorrectlyHandlesRemovalOfNonExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyError");

                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());

                validationContext.RemoveBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());
            }
        }

        [TestFixture]
        public class TheSynchronizeWithContextMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ValidationContext().SynchronizeWithContext(null));
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
                Assert.AreEqual(2, fieldValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldWarning", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, fieldValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldError", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, fieldValidations[counter].ValidationResultType);

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.AreEqual(2, businessRuleValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("BusinessRuleWarning", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, businessRuleValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("BusinessRuleError", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, businessRuleValidations[counter].ValidationResultType);
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
                Assert.AreEqual(2, fieldValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldError", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, fieldValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldWarning", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, fieldValidations[counter].ValidationResultType);

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.AreEqual(2, businessRuleValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("BusinessRuleWarning", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, businessRuleValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("BusinessRuleError", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, businessRuleValidations[counter].ValidationResultType);
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
                Assert.AreEqual(1, fieldValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldError", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, fieldValidations[counter].ValidationResultType);

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.AreEqual(2, businessRuleValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("BusinessRuleWarning", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, businessRuleValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("BusinessRuleError", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, businessRuleValidations[counter].ValidationResultType);
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
                Assert.AreEqual(2, fieldValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldWarning", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, fieldValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldError", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, fieldValidations[counter].ValidationResultType);

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.AreEqual(2, businessRuleValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("BusinessRuleWarning", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, businessRuleValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("BusinessRuleError", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, businessRuleValidations[counter].ValidationResultType);
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
                Assert.AreEqual(1, fieldValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldWarning", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, fieldValidations[counter].ValidationResultType);

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.AreEqual(2, businessRuleValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("BusinessRuleWarning", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, businessRuleValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("BusinessRuleError", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, businessRuleValidations[counter].ValidationResultType);
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
                Assert.AreEqual(2, fieldValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldWarning", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, fieldValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldError", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, fieldValidations[counter].ValidationResultType);

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.AreEqual(2, businessRuleValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("BusinessRuleError", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, businessRuleValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("BusinessRuleWarning", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, businessRuleValidations[counter].ValidationResultType);
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
                Assert.AreEqual(2, fieldValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldWarning", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, fieldValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldError", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, fieldValidations[counter].ValidationResultType);

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.AreEqual(1, businessRuleValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("BusinessRuleError", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, businessRuleValidations[counter].ValidationResultType);
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
                Assert.AreEqual(2, fieldValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldWarning", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, fieldValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldError", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, fieldValidations[counter].ValidationResultType);

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.AreEqual(2, businessRuleValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("BusinessRuleWarning", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, businessRuleValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("BusinessRuleError", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, businessRuleValidations[counter].ValidationResultType);
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
                Assert.AreEqual(2, fieldValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldWarning", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, fieldValidations[counter].ValidationResultType);

                counter++;
                Assert.AreEqual("MyProperty", fieldValidations[counter].PropertyName);
                Assert.AreEqual("FieldError", fieldValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Error, fieldValidations[counter].ValidationResultType);

                var businessRuleValidations = context1.GetBusinessRuleValidations();
                Assert.AreEqual(1, businessRuleValidations.Count);
                counter = -1;

                counter++;
                Assert.AreEqual("BusinessRuleWarning", businessRuleValidations[counter].Message);
                Assert.AreEqual(ValidationResultType.Warning, businessRuleValidations[counter].ValidationResultType);
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