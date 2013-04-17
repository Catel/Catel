// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationContextTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ValidationContextFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void AcceptsNullArguments()
            {
                var validationContext = new ValidationContext(null, null);

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());
                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());
            }
        }

        [TestClass]
        public class TheHasWarningsProperty
        {
            [TestMethod]
            public void ReturnsFalseForValidationContextWithoutWarnings()
            {
                var context = new ValidationContext();

                Assert.IsFalse(context.HasWarnings);
            }

            [TestMethod]
            public void ReturnsTrueForValidationContextWithFieldWarnings()
            {
                var context = CreateValidationContextSimple(true, false, false, false);

                Assert.IsTrue(context.HasWarnings);
            }

            [TestMethod]
            public void ReturnsTrueForValidationContextWithBusinessRuleWarnings()
            {
                var context = CreateValidationContextSimple(false, false, true, false);

                Assert.IsTrue(context.HasWarnings);
            }
        }

        [TestClass]
        public class TheHasErrorsProperty
        {
            [TestMethod]
            public void ReturnsFalseForValidationContextWithoutErrors()
            {
                var context = new ValidationContext();

                Assert.IsFalse(context.HasErrors);
            }

            [TestMethod]
            public void ReturnsTrueForValidationContextWithFieldErrors()
            {
                var context = CreateValidationContextSimple(false, true, false, false);

                Assert.IsTrue(context.HasErrors);
            }

            [TestMethod]
            public void ReturnsTrueForValidationContextWithBusinessRuleErrors()
            {
                var context = CreateValidationContextSimple(false, false, false, true);

                Assert.IsTrue(context.HasErrors);
            }
        }

        [TestClass]
        public class TheGetValidationCountMethod
        {
            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetValidationCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(8, context.GetValidationCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetValidationCount("tag"));
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetValidationCount("tag"));
            }
        }

        [TestClass]
        public class TheGetValidationsMethods
        {
            [TestMethod]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var validations = context.GetValidations();
                Assert.AreEqual(0, validations.Count);
            }

            [TestMethod]
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

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var validations = context.GetValidations("tag");
                Assert.AreEqual(0, validations.Count);
            }

            [TestMethod]
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

        [TestClass]
        public class TheGetWarningCount
        {
            [TestMethod]
            public void ReturnsZeroForContextWithoutWarnings()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetWarningCount());
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetWarningCount());
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetWarningCount("tag"));
                Assert.AreEqual(2, context.GetWarningCount(null));
            }
        }

        [TestClass]
        public class TheGetWarningsMethod
        {
            [TestMethod]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings();
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
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

            [TestMethod]
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

        [TestClass]
        public class TheGetErrorCount
        {
            [TestMethod]
            public void ReturnsZeroForContextWithoutErrors()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetWarningCount());
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetErrorCount());
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetErrorCount("tag"));
                Assert.AreEqual(2, context.GetErrorCount(null));
            }
        }

        [TestClass]
        public class TheGetErrorsMethod
        {
            [TestMethod]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors();
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
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

            [TestMethod]
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

        [TestClass]
        public class TheGetFieldValidationCountMethod
        {
            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldValidationCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetFieldValidationCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldValidationCount("tag"));
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetFieldValidationCount("tag"));
            }
        }

        [TestClass]
        public class TheGetFieldValidationsMethod
        {
            [TestMethod]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations();
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
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

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations((object)"tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations((object)"tag");
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("WarningWithTag", fieldValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[1].Message);
            }

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
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

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTagAndPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty", "tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTagAndPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldValidations("MyProperty", "tag");
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("WarningWithTag", fieldValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[1].Message);
            }
        }

        [TestClass]
        public class TheGetFieldWarningCountMethod
        {
            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldWarningCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetFieldWarningCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldWarningCount("tag"));
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(1, context.GetFieldWarningCount("tag"));
            }
        }

        [TestClass]
        public class TheGetFieldWarningsMethod
        {
            [TestMethod]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings();
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings();
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("WarningWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("WarningWithTag", fieldValidations[1].Message);
            }

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings((object)"tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings((object)"tag");
                Assert.AreEqual(1, fieldValidations.Count);
                Assert.AreEqual("WarningWithTag", fieldValidations[0].Message);
            }

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty");
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("WarningWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("WarningWithTag", fieldValidations[1].Message);
            }

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTagAndPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty", "tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTagAndPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldWarnings("MyProperty", "tag");
                Assert.AreEqual(1, fieldValidations.Count);
                Assert.AreEqual("WarningWithTag", fieldValidations[0].Message);
            }
        }

        [TestClass]
        public class TheGetFieldErrorCountMethod
        {
            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldErrorCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetFieldErrorCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetFieldErrorCount("tag"));
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(1, context.GetFieldErrorCount("tag"));
            }
        }

        [TestClass]
        public class TheGetFieldErrorsMethod
        {
            [TestMethod]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors();
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors();
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("ErrorWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[1].Message);
            }

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors("tag");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors((object)"tag");
                Assert.AreEqual(1, fieldValidations.Count);
                Assert.AreEqual("ErrorWithTag", fieldValidations[0].Message);
            }

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty");
                Assert.AreEqual(2, fieldValidations.Count);
                Assert.AreEqual("ErrorWithoutTag", fieldValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", fieldValidations[1].Message);
            }

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTagAndPropertyName()
            {
                var context = new ValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty");
                Assert.AreEqual(0, fieldValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTagAndPropertyName()
            {
                var context = CreateValidationContext();

                var fieldValidations = context.GetFieldErrors("MyProperty", "tag");
                Assert.AreEqual(1, fieldValidations.Count);
                Assert.AreEqual("ErrorWithTag", fieldValidations[0].Message);
            }
        }

        [TestClass]
        public class TheGetBusinessRuleValidationCountMethod
        {
            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleValidationCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(4, context.GetBusinessRuleValidationCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleValidationCount("tag"));
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetBusinessRuleValidationCount("tag"));
            }
        }

        [TestClass]
        public class TheGetBusinessRuleValidationsMethod
        {
            [TestMethod]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations();
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestMethod]
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

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations("tag");
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleValidations("tag");
                Assert.AreEqual(2, businessRuleValidations.Count);
                Assert.AreEqual("WarningWithTag", businessRuleValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", businessRuleValidations[1].Message);
            }
        }

        [TestClass]
        public class TheGetBusinessRuleWarningCountMethod
        {
            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleWarningCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetBusinessRuleWarningCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleWarningCount("tag"));
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(1, context.GetBusinessRuleWarningCount("tag"));
            }
        }

        [TestClass]
        public class TheGetBusinessRuleWarningsMethod
        {
            [TestMethod]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings();
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings();
                Assert.AreEqual(2, businessRuleValidations.Count);
                Assert.AreEqual("WarningWithoutTag", businessRuleValidations[0].Message);
                Assert.AreEqual("WarningWithTag", businessRuleValidations[1].Message);
            }

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings("tag");
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleWarnings("tag");
                Assert.AreEqual(1, businessRuleValidations.Count);
                Assert.AreEqual("WarningWithTag", businessRuleValidations[0].Message);
            }
        }

        [TestClass]
        public class TheGetBusinessRuleErrorCountMethod
        {
            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContext()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleErrorCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContext()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(2, context.GetBusinessRuleErrorCount());
            }

            [TestMethod]
            public void ReturnsRightAmountForEmptyValidationContextWithTag()
            {
                var context = new ValidationContext();

                Assert.AreEqual(0, context.GetBusinessRuleErrorCount("tag"));
            }

            [TestMethod]
            public void ReturnsRightAmountForFilledValidationContextWithTag()
            {
                var context = CreateValidationContext();

                Assert.AreEqual(1, context.GetBusinessRuleErrorCount("tag"));
            }
        }

        [TestClass]
        public class TheGetBusinessRuleErrorsMethod
        {
            [TestMethod]
            public void ReturnsRightValidationsForEmptyContext()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors();
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContext()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors();
                Assert.AreEqual(2, businessRuleValidations.Count);
                Assert.AreEqual("ErrorWithoutTag", businessRuleValidations[0].Message);
                Assert.AreEqual("ErrorWithTag", businessRuleValidations[1].Message);
            }

            [TestMethod]
            public void ReturnsRightValidationsForEmptyContextWithTag()
            {
                var context = new ValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors("tag");
                Assert.AreEqual(0, businessRuleValidations.Count);
            }

            [TestMethod]
            public void ReturnsRightValidationsForFilledContextWithTag()
            {
                var context = CreateValidationContext();

                var businessRuleValidations = context.GetBusinessRuleErrors("tag");
                Assert.AreEqual(1, businessRuleValidations.Count);
                Assert.AreEqual("ErrorWithTag", businessRuleValidations[0].Message);
            }
        }

        [TestClass]
        public class TheAddFieldValidationResultMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => validationContext.AddFieldValidationResult(null));
            }

            [TestMethod]
            public void AddsSingleValidationResultForField()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());

                validationContext.AddFieldValidationResult(fieldValidation);

                Assert.AreEqual(1, validationContext.GetFieldValidationCount());
            }

            [TestMethod]
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

        [TestClass]
        public class TheRemoveFieldValidationResultMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => validationContext.RemoveFieldValidationResult(null));
            }

            [TestMethod]
            public void CorrectlyHandlesRemovalOfExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                validationContext.AddFieldValidationResult(fieldValidation);

                Assert.AreEqual(1, validationContext.GetFieldValidationCount());

                validationContext.RemoveFieldValidationResult(fieldValidation);

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());
            }

            [TestMethod]
            public void CorrectlyHandlesRemovalOfNonExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var fieldValidation = FieldValidationResult.CreateError("MyProperty", "MyError");

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());

                validationContext.RemoveFieldValidationResult(fieldValidation);

                Assert.AreEqual(0, validationContext.GetFieldValidationCount());
            }
        }

        [TestClass]
        public class TheAddBusinessRuleValidationResultMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => validationContext.AddBusinessRuleValidationResult(null));
            }

            [TestMethod]
            public void AddsValidationResultOnce()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyProperty");

                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());

                validationContext.AddBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(1, validationContext.GetBusinessRuleValidationCount());
            }

            [TestMethod]
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

        [TestClass]
        public class TheRemoveBusinessRuleValidationMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                var validationContext = new ValidationContext();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => validationContext.RemoveBusinessRuleValidationResult(null));
            }

            [TestMethod]
            public void CorrectlyHandlesRemovalOfExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyError");

                validationContext.AddBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(1, validationContext.GetBusinessRuleValidationCount());

                validationContext.RemoveBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());
            }

            [TestMethod]
            public void CorrectlyHandlesRemovalOfNonExistingValidationResult()
            {
                var validationContext = new ValidationContext();
                var businessRuleValidation = BusinessRuleValidationResult.CreateError("MyError");

                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());

                validationContext.RemoveBusinessRuleValidationResult(businessRuleValidation);

                Assert.AreEqual(0, validationContext.GetBusinessRuleValidationCount());
            }
        }

        [TestClass]
        public class TheSynchronizeWithContextMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidationContext()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ValidationContext().SynchronizeWithContext(null));
            }

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
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