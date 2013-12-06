// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExposeAttribute.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    ///   Attribute to define a property available on a model property that is decorated with
    ///   the <see cref = "ModelAttribute" /> as well.
    ///   <para />
    ///   <example>
    ///     <code>
    ///       <![CDATA[
    ///       [Model]
    ///       [Expose("FirstName")]
    ///       [Expose("MiddleName")]
    ///       [Expose("LastName")]
    ///       public Person Person { get ;set; }
    ///       ]]>
    ///     </code>
    ///   </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    [ObsoleteEx(Replacement = "Moved to Catel.Fody. For more information, see https://catelproject.atlassian.net/browse/CTL-137", TreatAsErrorFromVersion = "3.9", RemoveInVersion = "4.0")]
    public class ExposeAttribute : Attribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "ExposeAttribute" /> class.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        /// <exception cref = "ArgumentException">The <paramref name = "propertyName" /> is <c>null</c> or whitespace.</exception>
        public ExposeAttribute(string propertyName)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            PropertyName = propertyName;
            PropertyNameOnModel = propertyName;
            Mode = ViewModelToModelMode.TwoWay;
        }

        /// <summary>
        ///   Gets the name of the property that should be automatically created.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        ///   Gets or sets the name of the property on the model. If the <see cref = "PropertyName" /> is not the
        ///   same as the name of the property on the model, this can be used to map the properties.
        ///   <para />
        ///   By default, the value is the same as the <see cref="PropertyName"/>.
        ///   <example>
        ///     In this example, the name of the property to map on the model is <c>first_name</c>, but
        ///     it must be available as <c>FirstName</c> on the view model.
        ///     <code>
        ///       <![CDATA[
        ///         [Model]
        ///         [Expose("FirstName", "first_name")]
        ///         public Person Person { get; set; }
        ///       ]]>
        ///     </code>
        ///   </example>
        /// </summary>
        /// <value>The property name on model.</value>
        public string PropertyNameOnModel { get; set; }

        /// <summary>
        /// Gets or sets the mode of the mapping.
        /// <para />
        /// The default value is <see cref="ViewModelToModelMode.TwoWay"/>.
        /// </summary>
        /// <value>The mode.</value>
        public ViewModelToModelMode Mode { get; set; }
    }
}