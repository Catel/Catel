// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateModelAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Data
{
    using System;

    /// <summary>
    /// Attribute to define custom validation at class level for all classes that derive from <see cref="ModelBase"/>.
    /// <para />
    /// This attribute follows a naming convention. If 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ValidateModelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateModelAttribute"/> class.
        /// </summary>
        /// <param name="validatorType">Type of the validator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validatorType"/> is <c>null</c>.</exception>
        public ValidateModelAttribute(Type validatorType)
        {
            Argument.IsNotNull("validatorType", validatorType);

            ValidatorType = validatorType;
        }

        /// <summary>
        /// Gets the type of the validator.
        /// </summary>
        /// <value>The type of the validator.</value>
        public Type ValidatorType { get; private set; }
    }
}