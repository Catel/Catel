﻿namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using Logging;
    using System.Reflection;
    using System.Text;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Model value class to store the mapping of the View Model to a Model mapping.
    /// </summary>
    public class ViewModelToModelMapping
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelToModelMapping"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="viewModelPropertyInfo">The view model property info.</param>
        /// <param name="modelPropertyType">The model property type.</param>
        /// <param name="attribute">The <see cref="ViewModelToModelAttribute"/> that was used to define the mapping.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewModelPropertyInfo"/> is <c>null</c> or whitespace.</exception>
        public ViewModelToModelMapping(IServiceProvider serviceProvider, PropertyInfo viewModelPropertyInfo, Type modelPropertyType, ViewModelToModelAttribute attribute)
            : this(serviceProvider, viewModelPropertyInfo.Name, viewModelPropertyInfo.PropertyType, modelPropertyType, attribute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelToModelMapping"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="viewModelProperty">The view model property.</param>
        /// <param name="viewModelPropertyType">The view model property type.</param>
        /// <param name="modelPropertyType">The model property type.</param>
        /// <param name="attribute">The <see cref="ViewModelToModelAttribute"/> that was used to define the mapping.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewModelProperty"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelPropertyType"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="modelPropertyType"/> is <c>null</c> or whitespace.</exception>
        public ViewModelToModelMapping(IServiceProvider serviceProvider, string viewModelProperty, Type viewModelPropertyType, Type modelPropertyType, ViewModelToModelAttribute attribute)
            : this(serviceProvider, viewModelProperty, viewModelPropertyType, attribute.Model, modelPropertyType, attribute.Property, attribute.Mode, attribute.ConverterType, attribute.AdditionalConstructorArgs, attribute.AdditionalPropertiesToWatch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelToModelMapping"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="viewModelProperty">The view model property.</param>
        /// <param name="viewModelPropertyType">The view model property type.</param>
        /// <param name="modelProperty">The model property.</param>
        /// <param name="modelPropertyType">The model property type.</param>
        /// <param name="valueProperty">The value property.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="converterType">Converter type</param>
        /// <param name="additionalConstructorArgs">Constructor args</param>
        /// <param name="additionalPropertiesToWatch"></param>
        /// <exception cref="ArgumentException">The <paramref name="viewModelProperty"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="modelPropertyType"/> is <c>null</c> or whitespace.</exception>
        public ViewModelToModelMapping(IServiceProvider serviceProvider, string viewModelProperty, Type viewModelPropertyType, string modelProperty, Type modelPropertyType, string valueProperty, 
            ViewModelToModelMode mode, Type converterType, object[] additionalConstructorArgs, string[] additionalPropertiesToWatch)
        {
            Argument.IsNotNullOrWhitespace("viewModelProperty", viewModelProperty);
            ArgumentNullException.ThrowIfNull(viewModelPropertyType);

            IgnoredProperties = new HashSet<string>();
            _serviceProvider = serviceProvider;
            ViewModelProperty = viewModelProperty;
            ViewModelPropertyType = viewModelPropertyType;
            ModelProperty = modelProperty;
            ModelPropertyType = modelPropertyType;
            Mode = mode;
            ConverterType = converterType;

            var propertiesLength = 1 + (additionalPropertiesToWatch is null ? 0 : additionalPropertiesToWatch.Length);
            ValueProperties = new string[propertiesLength]; ;
            ValueProperties[0] = valueProperty;

            if (propertiesLength > 1)
            {
                additionalPropertiesToWatch?.CopyTo(ValueProperties, 1);
            }

            var argsLength = 1 + (additionalConstructorArgs is null ? 0 : additionalConstructorArgs.Length);
            var args = new object[argsLength];
            args[0] = ValueProperties;

            if (argsLength > 1)
            {
                additionalConstructorArgs?.CopyTo(args, 1);
            }

            var converter = ActivatorUtilities.CreateInstance(_serviceProvider, ConverterType, args) as IViewModelToModelConverter;
            if (converter is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Failed to create converter '{ConverterType}'");
            }

            Converter = converter;
        }

        /// <summary>
        /// Gets the ignored properties.
        /// </summary>
        /// <value>
        /// The ignored properties.
        /// </value>
        public HashSet<string> IgnoredProperties
        {
            get; private set;
        }

        /// <summary>
        /// Gets the property name of the mapping of the view model.
        /// </summary>
        /// <value>The model view property.</value>
        public string ViewModelProperty { get; private set; }

        /// <summary>
        /// Gets the property type of the mapping of the view model.
        /// </summary>
        /// <value>The model view property type.</value>
        public Type ViewModelPropertyType { get; private set; }

        /// <summary>
        /// Gets the property name of the model.
        /// </summary>
        /// <value>The model.</value>
        public string ModelProperty { get; private set; }

        /// <summary>
        /// Gets the property type of the mapping of the  model.
        /// </summary>
        /// <value>The model property type.</value>
        public Type ModelPropertyType { get; private set; }

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

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"ViewModel.{ModelProperty}.{ViewModelProperty}");

            switch (Mode)
            {
                case ViewModelToModelMode.Explicit:
                    stringBuilder.Append(" <= explicit => ");
                    break;

                case ViewModelToModelMode.OneWay:
                    stringBuilder.Append(" <= ");
                    break;

                case ViewModelToModelMode.OneWayToSource:
                    stringBuilder.Append(" => ");
                    break;

                case ViewModelToModelMode.TwoWay:
                    stringBuilder.Append(" <=> ");
                    break;
            }

            var property = ValueProperties.FirstOrDefault();
            if (ValueProperties.Length > 1)
            {
                property += $"+ {ValueProperties.Length}";
            }

            stringBuilder.Append($"{ModelPropertyType.Name}.{property}");

            return stringBuilder.ToString();
        }
    }
}
