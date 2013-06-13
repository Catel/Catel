// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelMetadata.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Class containing meta data for a view model type.
    /// </summary>
    internal class ViewModelMetadata
    {
#if NET
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelMetadata"/> class.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="models">The models.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="validations">The validations.</param>
        /// <param name="propertyDescriptors">The property descriptors.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="models"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="mappings"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyDescriptors"/> is <c>null</c>.</exception>
        internal ViewModelMetadata(Type viewModelType, Dictionary<string, ModelInfo> models, Dictionary<string, ViewModelToModelMapping> mappings,
            Dictionary<string, ValidationToViewModelAttribute> validations, IEnumerable<ViewModelPropertyDescriptor> propertyDescriptors)
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelMetadata"/> class.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="models">The models.</param>
        /// <param name="mappings">The mappings.</param>
        /// <param name="validations">The validations.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="models"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="mappings"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validations"/> is <c>null</c>.</exception>
        internal ViewModelMetadata(Type viewModelType, Dictionary<string, ModelInfo> models, Dictionary<string, ViewModelToModelMapping> mappings,
            Dictionary<string, ValidationToViewModelAttribute> validations)
#endif
        {
            Argument.IsNotNull("viewModelType", viewModelType);
            Argument.IsNotNull("models", models);
            Argument.IsNotNull("mappings", mappings);
            Argument.IsNotNull("validations", validations);

#if NET
            Argument.IsNotNull("propertyDescriptors", propertyDescriptors);
#endif

            ViewModelType = viewModelType;
            Models = models;
            Mappings = mappings;
            Validations = validations;

#if NET
            PropertyDescriptors = propertyDescriptors;
#endif
        }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Gets the models.
        /// </summary>
        public Dictionary<string, ModelInfo> Models { get; private set; }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        public Dictionary<string, ViewModelToModelMapping> Mappings { get; private set; }

        /// <summary>
        /// Gets the validations.
        /// </summary>
        public Dictionary<string, ValidationToViewModelAttribute> Validations { get; private set; }

#if NET
        /// <summary>
        /// Gets the property descriptors.
        /// </summary>
        public IEnumerable<ViewModelPropertyDescriptor> PropertyDescriptors { get; private set; } 
#endif
    }
}