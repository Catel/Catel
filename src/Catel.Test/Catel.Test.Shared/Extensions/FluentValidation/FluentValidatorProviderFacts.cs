// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentValidatorProviderFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Test.Extensions.FluentValidation
{
    using Catel.Data;
    using Catel.IoC;
    using Catel.Test.Extensions.FluentValidation.Models;
    using Catel.Test.Extensions.FluentValidation.ViewModels;

    using NUnit.Framework;

    /// <summary>
    /// The person view model test fixture.
    /// </summary>
    public class FluentValidatorProviderFacts
    {
        /// <summary>
        /// The fluent validator provider test.
        /// </summary>
        [TestFixture]
        public class FluentValidatorProviderTest
        {
            [SetUp]
            public void SetUp()
            {
                ServiceLocator.Default.RegisterType<IValidatorProvider, FluentValidatorProvider>();
            }

            /// <summary>
            /// The validation 
            /// </summary>
            [TestCase]
            public void ModelBaseWithFieldValidationTest()
            {
                var personViewModel = new PersonViewModelWithModel { Person = new Person { FirstName = "Igr Alexánder", LastName = string.Empty } };
                
                // I have to add this call here
                ((IModelValidation)personViewModel).Validate();

                var validationSummary = personViewModel.GetValidationContext().GetValidationSummary("Person");

                Assert.IsTrue(validationSummary.HasErrors);
            }

            /// <summary>
            /// The person view model no errors test.
            /// </summary>
            [TestCase]
            public void ViewModelBaseNoErrorsTest()
            {
                var personViewModel = new PersonViewModel { PersonFirstName = "Igr Alexánder", PersonLastName = "Fernández Saúco" };

                ((IModelValidation)personViewModel).Validate();

                var validationSummary = personViewModel.GetValidationContext().GetValidationSummary("Person");

                Assert.IsFalse(validationSummary.HasErrors);
                Assert.IsFalse(validationSummary.HasWarnings);
            }

            /// <summary>
            /// The person view model with field errors and business rule warnings test.
            /// </summary>
            [TestCase]
            public void ViewModelBaseWithFieldErrorsAndBusinessRuleWarningsValidationTest()
            {
                var personViewModel = new PersonViewModel();
                ((IModelValidation)personViewModel).Validate();

                var validationSummary = personViewModel.GetValidationContext().GetValidationSummary("Person");

                Assert.IsTrue(validationSummary.HasErrors);
                Assert.IsTrue(validationSummary.HasFieldErrors);
                Assert.AreEqual(2, validationSummary.FieldErrors.Count);
                Assert.IsTrue(validationSummary.FieldErrors[0].Message.Contains("First name"));
                Assert.IsTrue(validationSummary.FieldErrors[1].Message.Contains("Last name"));

                Assert.IsTrue(validationSummary.HasWarnings);
                Assert.IsTrue(validationSummary.HasBusinessRuleWarnings);
                Assert.AreEqual(2, validationSummary.BusinessRuleWarnings.Count);
                Assert.IsTrue(validationSummary.BusinessRuleWarnings[0].Message.Contains("First name"));
                Assert.IsTrue(validationSummary.BusinessRuleWarnings[1].Message.Contains("Last name"));
            }

            /// <summary>
            /// The person view with out validator test.
            /// </summary>
            [TestCase]
            public void PersonViewWithOutValidatorTest()
            {
                var validatorProvider = ServiceLocator.Default.ResolveType<IValidatorProvider>();
                IValidator validator = validatorProvider.GetValidator(typeof(NoFluentValidatorViewModel));

                Assert.IsNull(validator);
            }
        }

        /// <summary>
        /// The the cache usage.
        /// </summary>
        [TestFixture]
        public class GetValidatorMethod
        {
            /// <summary>
            /// The init.
            /// </summary>
            [SetUp]
            public void SetUp()
            {
                ServiceLocator.Default.RegisterType<IValidatorProvider, FluentValidatorProvider>();
            }

            /// <summary>
            /// The person view with out validator test.
            /// </summary>
            [TestCase]
            public void MustReturnsTheSameInstanceIfCacheIsActive()
            {
                var validatorProvider = ServiceLocator.Default.ResolveType<IValidatorProvider>();

                IValidator validator1 = validatorProvider.GetValidator(typeof(PersonViewModel));
                Assert.IsNotNull(validator1);

                IValidator validator2 = validatorProvider.GetValidator(typeof(PersonViewModel));
                Assert.AreEqual(validator1, validator2);
            }

            /// <summary>
            /// The returns different instances if it turned off.
            /// </summary>
            [TestCase]
            public void ReturnsDifferentInstancesIfCacheTurnedOff()
            {
                var validatorProvider = ServiceLocator.Default.ResolveType<IValidatorProvider>();
                IValidator validator1 = validatorProvider.GetValidator(typeof(PersonViewModel));
                Assert.IsNotNull(validator1);
                ((ValidatorProviderBase)validatorProvider).UseCache = false;
                IValidator validator2 = validatorProvider.GetValidator(typeof(PersonViewModel));
                Assert.AreNotEqual(validator1, validator2);
            }
        }
    }
}

#endif