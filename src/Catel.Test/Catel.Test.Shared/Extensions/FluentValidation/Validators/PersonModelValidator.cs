// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonModelValidator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Test.Extensions.FluentValidation.Validators
{
    using Catel.Test.Extensions.FluentValidation.Models;

    using global::FluentValidation;

    /// <summary>
    /// The person model validator.
    /// </summary>
    public class PersonModelValidator : AbstractValidator<Person>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonModelValidator"/> class.
        /// </summary>
        public PersonModelValidator()
        {
            RuleFor(person => person.FirstName).NotNull().NotEmpty();
            RuleFor(person => person.LastName).NotNull().NotEmpty();
        }

        #endregion
    }
}

#endif