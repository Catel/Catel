// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonViewModelWithModelValidator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.FluentValidation.Validators
{
    using Catel.Test.Extensions.FluentValidation.ViewModels;

    using global::FluentValidation;

    /// <summary>
    /// The person view model with model validator.
    /// </summary>
    [ValidatorDescription("Person")]
    public class PersonViewModelWithModelValidator : AbstractValidator<PersonViewModelWithModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonViewModelWithModelValidator"/> class.
        /// </summary>
        public PersonViewModelWithModelValidator()
        {
            RuleFor(model => model.Person).SetValidator(new PersonModelValidator());
        }

        #endregion
    }
}