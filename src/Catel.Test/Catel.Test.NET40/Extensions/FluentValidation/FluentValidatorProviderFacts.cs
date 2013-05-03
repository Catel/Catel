// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentValidatorProviderFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.FluentValidation
{
    using Catel.Data;
    using Catel.IoC;
    using Catel.Test.Extensions.FluentValidation.Models;
    using Catel.Test.Extensions.FluentValidation.ViewModels;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    /// The person view model test fixture.
    /// </summary>
    public class FluentValidatorProviderFacts
    {
        /// <summary>
        /// The fluent validator provider test.
        /// </summary>
        [TestClass]
        public class FluentValidatorProviderTest
        {
            [TestInitialize]
            public void Init()
            {
                ServiceLocator.Default.RegisterType<IValidatorProvider, FluentValidatorProvider>();
            }

            /// <summary>
            /// The validation 
            /// </summary>
            [TestMethod]
            public void ModelBaseWithFieldValidationTest()
            {
                var personViewModel = new PersonViewModelWithModel { Person = new Person { FirstName = "Igr Alexánder", LastName = string.Empty } };
                
                // I have to add this call here
                personViewModel.Validate();
                
                IValidationSummary validationSummary = personViewModel.ValidationContext.GetValidationSummary("Person");
                Assert.IsTrue(validationSummary.HasErrors);
            }

            /// <summary>
            /// The person view model no errors test.
            /// </summary>
            [TestMethod]
            public void ViewModelBaseNoErrorsTest()
            {
                var personViewModel = new PersonViewModel { PersonFirstName = "Igr Alexánder", PersonLastName = "Fernández Saúco" };

                personViewModel.Validate();

                IValidationSummary validationSummary = personViewModel.ValidationContext.GetValidationSummary("Person");
                Assert.IsFalse(validationSummary.HasErrors);
                Assert.IsFalse(validationSummary.HasWarnings);
            }

            /// <summary>
            /// The person view model with field errors and business rule warnings test.
            /// </summary>
            [TestMethod]
            public void ViewModelBaseWithFieldErrorsAndBusinessRuleWarningsValidationTest()
            {
                var personViewModel = new PersonViewModel();
                personViewModel.Validate();

                IValidationSummary validationSummary = personViewModel.ValidationContext.GetValidationSummary("Person");

                Assert.IsTrue(validationSummary.HasErrors);
                Assert.IsTrue(validationSummary.HasFieldErrors);
                Assert.AreEqual(2, validationSummary.FieldErrors.Count);
                Assert.IsTrue(validationSummary.FieldErrors[0].Message.Contains("First name"));
                Assert.IsTrue(validationSummary.FieldErrors[1].Message.Contains("Last name"));

                Assert.IsTrue(validationSummary.HasWarnings);
                Assert.IsTrue(validationSummary.HasBusinessRuleWarnings);
                Assert.AreEqual(2, validationSummary.BusinessWarnings.Count);
                Assert.IsTrue(validationSummary.BusinessWarnings[0].Message.Contains("First name"));
                Assert.IsTrue(validationSummary.BusinessWarnings[1].Message.Contains("Last name"));
            }

            /// <summary>
            /// The person view with out validator test.
            /// </summary>
            [TestMethod]
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
        [TestClass]
        public class GetValidatorMethod
        {
            /// <summary>
            /// The init.
            /// </summary>
            [TestInitialize]
            public void Init()
            {
                ServiceLocator.Default.RegisterType<IValidatorProvider, FluentValidatorProvider>();
            }

            /// <summary>
            /// The person view with out validator test.
            /// </summary>
            [TestMethod]
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
            [TestMethod]
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