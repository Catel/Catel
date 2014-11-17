// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelToModelMapping.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using Windows.Interactivity.DragDropHelpers;
    using IoC;

    /// <summary>
    /// Model value class to store the mapping of the View Model to a Model mapping.
    /// </summary>
    public class ViewModelToModelMapping
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelToModelMapping"/> class.
        /// </summary>
        /// <param name="viewModelProperty">The view model property.</param>
        /// <param name="attribute">The <see cref="ViewModelToModelAttribute"/> that was used to define the mapping.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewModelProperty"/> is <c>null</c> or whitespace.</exception>
        public ViewModelToModelMapping(string viewModelProperty, ViewModelToModelAttribute attribute)
            : this(viewModelProperty, attribute.Model, attribute.Property, attribute.Mode, attribute.ConverterType, attribute.AdditionalConstructorArgs, attribute.AdditionalPropertiesToWatch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelToModelMapping"/> class.
        /// </summary>
        /// <param name="viewModelProperty">The view model property.</param>
        /// <param name="modelProperty">The model property.</param>
        /// <param name="valueProperty">The value property.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="converterType">Converter type</param>
        /// <param name="additionalConstructorArgs">Constructor args</param>
        /// <param name="additionalPropertiesToWatch"></param>
        /// <exception cref="ArgumentException">The <paramref name="viewModelProperty"/> is <c>null</c> or whitespace.</exception>
        public ViewModelToModelMapping(string viewModelProperty, string modelProperty, string valueProperty, ViewModelToModelMode mode, Type converterType, object[] additionalConstructorArgs, string[] additionalPropertiesToWatch)
        {
            Argument.IsNotNullOrWhitespace("viewModelProperty", viewModelProperty);

            ViewModelProperty = viewModelProperty;
            ModelProperty = modelProperty;
            Mode = mode;
            ConverterType = converterType;

            var propertiesLength = 1 + (additionalPropertiesToWatch == null ? 0 : additionalPropertiesToWatch.Length);
            ValueProperties = new string[propertiesLength]; ;
            ValueProperties[0] = valueProperty;

            if (propertiesLength > 1)
                additionalPropertiesToWatch.CopyTo(ValueProperties, 1);

            var argsLength = 1 + (additionalConstructorArgs == null ? 0 : additionalConstructorArgs.Length);
            var args = new object[argsLength];
            args[0] = ValueProperties;

            if (argsLength > 1)
                additionalConstructorArgs.CopyTo(args, 1);

            Converter = (IViewModelToModelConverter)this.GetTypeFactory().CreateInstanceWithParameters(ConverterType, args);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the property name of the mapping of the view model.
        /// </summary>
        /// <value>The model view property.</value>
        public string ViewModelProperty { get; private set; }

        /// <summary>
        /// Gets the property name of the model.
        /// </summary>
        /// <value>The model.</value>
        public string ModelProperty { get; private set; }

        /// <summary>
        /// Gets the property property name of the property in the model.
        /// </summary>
        /// <value>The property.</value>
        public string[] ValueProperties { get; private set; }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public ViewModelToModelMode Mode { get; private set; }

        /// <summary>
        /// Gets the type of the converter.
        /// <para />
        /// The default value is <see cref="ViewModelToModelMode.TwoWay"/>.
        /// </summary>
        /// <value>The converter type.</value>
        public Type ConverterType { get; private set; }

        /// <summary>
        /// Gets the converter.
        /// <para />
        /// The default value is <see cref="ViewModelToModelMode.TwoWay"/>.
        /// </summary>
        /// <value>The converter.</value>
        public IViewModelToModelConverter Converter { get; private set; }
        #endregion
    }
}