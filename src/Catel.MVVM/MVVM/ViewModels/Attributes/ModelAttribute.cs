namespace Catel.MVVM
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Attribute to define a property in a view model as a model. Objects decorated with this attribute
    /// will be automatically validated when a property changes.
    /// </summary>
    public class ModelAttribute : Attribute
    {
        static ModelAttribute()
        {
            SupportIEditableObjectDefaultValue = true;
            SupportValidationDefaultValue = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelAttribute"/> class.
        /// </summary>
        public ModelAttribute()
        {
            SupportIEditableObject = SupportIEditableObjectDefaultValue;
            SupportValidation = SupportValidationDefaultValue;
        }

        /// <summary>
        /// Gets or sets the default value to use for the <see cref="SupportIEditableObject"/> value.
        /// </summary>
        public static bool SupportIEditableObjectDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IEditableObject"/> interface should be used on the model if possible.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="IEditableObject"/> interface should be used on the model if possible; otherwise, <c>false</c>.
        /// </value>
        public bool SupportIEditableObject { get; set; }

        /// <summary>
        /// Gets or sets the default value to use for the <see cref="SupportValidation"/> value.
        /// </summary>
        public static bool SupportValidationDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the model validation mapping is enabled.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the model validation mapping is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool SupportValidation { get; set; }
    }
}
