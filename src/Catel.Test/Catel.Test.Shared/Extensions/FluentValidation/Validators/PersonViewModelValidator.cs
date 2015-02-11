// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonViewModelValidator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Test.Extensions.FluentValidation.Validators
{
    using Catel.Data;

    using global::FluentValidation;

    using ViewModels;

    using NUnit.Framework;

    /// <summary>
    /// The person view model validator.
    /// </summary>
    [ValidatorDescription("Person")]
    public class PersonViewModelValidator : AbstractValidator<PersonViewModel>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonViewModelValidator"/> class.
        /// </summary>
        public PersonViewModelValidator()
        {
            RuleFor(model => model.PersonFirstName).NotNull().NotEmpty();
            RuleFor(model => model.PersonLastName).NotNull().NotEmpty();
        }

        #endregion
    }

    /// <summary>
    /// The person view model validator warnings.
    /// </summary>
    [ValidatorDescription("Person", ValidationResultType.Warning, ValidationType.BusinessRule)]
    public class PersonViewModelValidatorWarnings : AbstractValidator<PersonViewModel>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonViewModelValidatorWarnings"/> class.
        /// </summary>
        public PersonViewModelValidatorWarnings()
        {
            RuleFor(model => model.PersonFirstName).NotNull().Length(3, 20);
            RuleFor(model => model.PersonLastName).NotNull().Length(3, 20);
        }

        #endregion
    }

    public class JustAnotherPersonViewModelValidatorButDoNothing : AbstractValidator<PersonViewModel>
    {
    }
}

#endif