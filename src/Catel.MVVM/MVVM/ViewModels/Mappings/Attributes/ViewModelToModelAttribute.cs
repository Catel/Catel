namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Attribute to link a property in a view model to a model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ViewModelToModelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelToModelAttribute"/> class.
        /// </summary>
        /// <param name="model">The property name that holds the model object.</param>
        /// <param name="property">The property of the model object that should be linked to the <see cref="ViewModelBase"/> property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        public ViewModelToModelAttribute(string model = "", string property = "")
        {
            ArgumentNullException.ThrowIfNull(model);
            Argument.IsNotNull("property", property);

            Model = model;
            Property = property;

            Mode = ViewModelToModelMode.TwoWay;
            ConverterType = typeof(DefaultViewModelToModelMappingConverter);
        }

        /// <summary>
        /// Gets the property name that holds the model object.
        /// <para />
        /// Must be a property on the <see cref="ViewModelBase"/> implementation, but is allowed to be private.
        /// </summary>
        /// <value>The model property name.</value>
        public string Model { get; internal set; }

        /// <summary>
        /// Gets the property of the model object that should be linked to the <see cref="ViewModelBase"/> property.
        /// </summary>
        /// <value>The property.</value>
        public string Property { get; internal set; }

        /// <summary>
        /// Gets or sets the mode of the mapping.
        /// <para />
        /// The default value is <see cref="ViewModelToModelMode.TwoWay"/>.
        /// </summary>
        /// <value>The mode.</value>
        public ViewModelToModelMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the type of the converter.
        /// <para />
        /// The default value is <see cref="ViewModelToModelMode.TwoWay"/>.
        /// </summary>
        /// <value>The converter.</value>
        public Type ConverterType { get; set; }

        /// <summary>
        /// Gets or sets the additional constructor args.
        /// <para />
        /// This args would be passed to constructor.
        /// </summary>
        /// <value>The additional constructor args.</value>
        public object[] AdditionalConstructorArgs { get; set; }

        /// <summary>
        /// Gets or sets the additional properties to triger converter.
        /// <para />
        /// This args would be passed to constructor.
        /// </summary>
        /// <value>The additional properties to watch.</value>
        public string[] AdditionalPropertiesToWatch { get; set; }
    }
}
