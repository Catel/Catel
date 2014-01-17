// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelPropertyDescriptorFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Factory for <see cref="ViewModelPropertyDescriptor"/> instances. This way, a property descriptor can be re-used without
    /// having to use reflection.
    /// </summary>
    internal static class ViewModelPropertyDescriptorFactory
    {
        /// <summary>
        /// Creates a property descriptor for a specific view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns>The <see cref="ViewModelPropertyDescriptor" />.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyType" /> is <c>null</c>.</exception>
        public static ViewModelPropertyDescriptor CreatePropertyDescriptor(ViewModelBase viewModel, string propertyName, Type propertyType, Attribute[] attributes)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("propertyType", propertyType);

            if (attributes == null)
            {
                attributes = new Attribute[] { };
            }

            return new ViewModelPropertyDescriptor(viewModel, propertyName, propertyType, attributes);
        }

        /// <summary>
        /// Creates the property descriptor for a specfic view model based on an existing property descriptor.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>The <see cref="ViewModelPropertyDescriptor" />.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyDescriptor" /> is <c>null</c>.</exception>
        public static ViewModelPropertyDescriptor CreatePropertyDescriptor(ViewModelBase viewModel, ViewModelPropertyDescriptor propertyDescriptor)
        {
            Argument.IsNotNull("propertyDescriptor", propertyDescriptor);

            var attributes = new List<Attribute>();
            if (propertyDescriptor.Attributes != null)
            {
                foreach (var attribute in propertyDescriptor.Attributes)
                {
                    var attrib = attribute as Attribute;
                    if (attrib != null)
                    {
                        attributes.Add(attrib);
                    }
                }
            }

            return CreatePropertyDescriptor(viewModel, propertyDescriptor.Name, propertyDescriptor.PropertyType, attributes.ToArray());
        }
    }
}