// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDependencyExtension.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Markup
{
    using System;
    using System.Windows.Markup;
    using IoC;

    /// <summary>
    /// Service dependency extension to allow service access in xaml for services with properties.
    /// </summary>
    public class ServiceDependencyExtension : MarkupExtension
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of a class derived from <see cref="T:System.Windows.Markup.MarkupExtension"/>.
        /// </summary>
        public ServiceDependencyExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDependencyExtension"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ServiceDependencyExtension(Type type)
        {
            Type = type;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConstructorArgument("type")]
        public Type Type { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Catel.Environment.IsInDesignMode)
            {
                return null;
            }

            return ServiceLocator.Default.ResolveType(Type);
        }
        #endregion
    }
}