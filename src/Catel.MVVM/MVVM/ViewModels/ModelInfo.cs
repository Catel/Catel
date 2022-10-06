namespace Catel.MVVM
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// Class containing information about a specific model decorated with the <see cref="ModelAttribute"/>.
    /// </summary>
    internal class ModelInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property info of the model property.</param>
        /// <param name="attribute">The attribute.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="attribute"/> is <c>null</c>.</exception>
        public ModelInfo(PropertyInfo propertyInfo, ModelAttribute attribute)
            : this(propertyInfo.Name, propertyInfo.PropertyType, attribute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the model property.</param>
        /// <param name="propertyType">The property type of the model property.</param>
        /// <param name="attribute">The attribute.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="attribute"/> is <c>null</c>.</exception>
        public ModelInfo(string name, Type propertyType, ModelAttribute attribute)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            ArgumentNullException.ThrowIfNull(propertyType);
            ArgumentNullException.ThrowIfNull(attribute);

            Name = name;
            PropertyType = propertyType;
            SupportIEditableObject = attribute.SupportIEditableObject;
            SupportValidation = attribute.SupportValidation;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the model validation mapping is enabled.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the model validation mapping is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool SupportValidation { get; set; }

        /// <summary>
        /// Gets the name of the model property.
        /// </summary>
        /// <value>The name of the model property.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IEditableObject"/> interface should be used on the model if possible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="IEditableObject"/> interface should be used on the model if possible; otherwise, <c>false</c>.
        /// </value>
        public bool SupportIEditableObject { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this model is canceling.
        /// </summary>
        /// <value><c>true</c> if this model is canceling; otherwise, <c>false</c>.</value>
        public bool IsCanceling { get; set; }
    }
}
