// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationToViewModelAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using Data;

    /// <summary>
    /// Attribute to gather validation from a <see cref="IValidationContext"/> in a property, which must be of type
    /// <see cref="IValidationSummary"/>.
    /// </summary>
    /// <example>
    /// The attribute must be used like this and all validations with the tag <c>PersonValidationTag</c> will 
    /// be gathered into the summary:
    /// <para />
    /// <code>
    /// <![CDATA[
    /// [ValidationToViewModel("PersonValidationTag")]
    /// public IValidationSummary PersonValidationSummary { get; set; }
    /// ]]>
    /// </code>
    /// </example>
    /// <remarks>
    /// This attribute can only be used inside a view model.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidationToViewModelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationToViewModelAttribute"/> class.
        /// </summary>
        public ValidationToViewModelAttribute()
        {
            IncludeChildViewModels = false;
            UseTagToFilter = true;
            Tag = null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the validation of the child view models should also be included.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the validation of the child view models should also be included; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeChildViewModels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this attribute should use the tag to filter the validation.
        /// <para />
        /// If the value is <c>true</c>, the validation will be filtered on the tag. Otherwise, all validation results 
        /// will be returned. Keep in mind that the <see cref="Tag"/> can still be <c>null</c>, even when this value is <c>true</c>.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if [use tag to filter]; otherwise, <c>false</c>.</value>
        public bool UseTagToFilter { get; set; }

        /// <summary>
        /// Gets or sets the tag to filter by.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }
    }
}
