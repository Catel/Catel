// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewToViewModelMapping.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;
    using Logging;

    /// <summary>
    /// Holds the information for a view to viewmodel mapping. The information is based on the <see cref="ViewToViewModelAttribute"/>.
    /// </summary>
    internal class ViewToViewModelMapping
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewToViewModelMapping"/> class.
        /// </summary>
        /// <param name="viewPropertyName">Name of the view property.</param>
        /// <param name="viewModelPropertyName">Name of the view model property.</param>
        /// <param name="mapping">The mapping type.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewPropertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelPropertyName"/> is <c>null</c> or whitespace.</exception>
        public ViewToViewModelMapping(string viewPropertyName, string viewModelPropertyName, ViewToViewModelMappingType mapping)
        {
            Argument.IsNotNullOrWhitespace("viewPropertyName", viewPropertyName);
            Argument.IsNotNullOrWhitespace("viewModelPropertyName", viewModelPropertyName);

            ViewPropertyName = viewPropertyName;
            ViewModelPropertyName = viewModelPropertyName;
            MappingType = mapping;

            Log.Debug("Created a '{0}' view to viewmodel mapping from '{1}' to '{2}'", mapping, viewPropertyName, viewModelPropertyName);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the view property.
        /// </summary>
        /// <value>The name of the view property.</value>
        public string ViewPropertyName { get; private set; }

        /// <summary>
        /// Gets the name of the view model property.
        /// </summary>
        /// <value>The name of the view model property.</value>
        public string ViewModelPropertyName { get; private set; }

        /// <summary>
        /// Gets or sets the mapping type.
        /// </summary>
        /// <value>The mapping type.</value>
        public ViewToViewModelMappingType MappingType { get; private set; }
        #endregion
    }
}