// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelToModelAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Specifies the different mapping modes available for the <see cref="ViewModelToModelAttribute" />.
    /// </summary>
    public enum ViewModelToModelMode
    {
        /// <summary>
        /// Automatically maps the property from view model to model and back as soon
        /// as either one changes the property value.
        /// </summary>
        TwoWay,

        /// <summary>
        /// Automatically maps the property from the model to the view model if the model
        /// changes the property value.
        /// <para />
        /// This mode does not map any values from the view model to the model, thus can also
        /// be seen as read-only mode.
        /// </summary>
        OneWay,

        /// <summary>
        /// Automatically maps the property from the view model to the model if the view model
        /// changes the property value.
        /// <para />
        /// This mode does not map any values from the model to the view model, but still keeps track
        /// of all validation that occurs in the model.
        /// </summary>
        OneWayToSource,

        /// <summary>
        /// Automatically maps properties from the model to the view model as soon as the is initialized. As 
        /// soon as a property value changes in the model, the view model value is updated instantly. However,
        /// the mapping from the view model to model is explicit.
        /// </summary>
        Explicit
    }

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
		/// <exception cref="ArgumentException">The <paramref name="model"/> is <c>null</c> or whitespace.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
		public ViewModelToModelAttribute(string model, string property = "")
		{
            Argument.IsNotNullOrWhitespace("model", model);
            Argument.IsNotNull("property", property);

			Model = model;
			Property = property;

            Mode = ViewModelToModelMode.TwoWay;
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
	}
}