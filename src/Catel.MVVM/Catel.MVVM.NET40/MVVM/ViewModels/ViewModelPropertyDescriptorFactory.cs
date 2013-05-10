// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelPropertyDescriptorFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

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
        /// <returns>The <see cref="ViewModelPropertyDescriptor"/>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyType"/> is <c>null</c>.</exception>
        public static ViewModelPropertyDescriptor CreatePropertyDescriptor(ViewModelBase viewModel, string propertyName, Type propertyType)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("propertyType", propertyType);

            return new ViewModelPropertyDescriptor(viewModel, propertyName, propertyType);
        }

        /// <summary>
        /// Creates the property descriptor for a specfic view model based on an existing property descriptor. 
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>The <see cref="ViewModelPropertyDescriptor"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyDescriptor"/> is <c>null</c>.</exception>
        public static ViewModelPropertyDescriptor CreatePropertyDescriptor(ViewModelBase viewModel, ViewModelPropertyDescriptor propertyDescriptor)
        {
            Argument.IsNotNull("propertyDescriptor", propertyDescriptor);

            return CreatePropertyDescriptor(viewModel, propertyDescriptor.Name, propertyDescriptor.PropertyType);
        }
    }
}