// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatorDescriptionAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;

    using Catel.Data;

    /// <summary>
    /// The validator description attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ValidatorDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorDescriptionAttribute" /> class.
        /// </summary>
        /// <param name="tag">The validation tag.</param>
        /// <param name="validationResultType">The validation result type.</param>
        /// <param name="validationType">The validation type.</param>
        public ValidatorDescriptionAttribute(string tag, ValidationResultType validationResultType = ValidationResultType.Error,
            ValidationType validationType = ValidationType.Field)
        {
            Tag = tag;
            ValidationResultType = validationResultType;
            ValidationType = validationType;
        }

        /// <summary>
        /// Gets the validation tag.
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// Gets the validation result type.
        /// </summary>
        public ValidationResultType ValidationResultType { get; private set; }

        /// <summary>
        /// Gets the validation type.
        /// </summary>
        public ValidationType ValidationType { get; private set; }
    }
}