// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewToViewModelMappingContainer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Logging;
    using Reflection;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
    /// Container class for <see cref="ViewToViewModelMapping"/> elements.
    /// </summary>
    internal class ViewToViewModelMappingContainer
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dictionary containing all the view to view model mappings.
        /// </summary>
        private readonly Dictionary<string, ViewToViewModelMapping> _viewToViewModelMappings = new Dictionary<string, ViewToViewModelMapping>();

        /// <summary>
        /// Dictionary containing all the view model to view mappings.
        /// </summary>
        private readonly Dictionary<string, ViewToViewModelMapping> _viewModelToViewMappings = new Dictionary<string, ViewToViewModelMapping>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewToViewModelMappingContainer"/> class.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyObject"/> is <c>null</c>.</exception>
        public ViewToViewModelMappingContainer(DependencyObject dependencyObject)
        {
            Argument.IsNotNull("dependencyObject", dependencyObject);

            var properties = dependencyObject.GetType().GetPropertiesEx();
            foreach (var property in properties)
            {
                object[] viewToViewModelAttributes = property.GetCustomAttributesEx(typeof(ViewToViewModelAttribute), false);
                if (viewToViewModelAttributes.Length > 0)
                {
                    Log.Debug("Property '{0}' is decorated with the ViewToViewModelAttribute, creating a mapping", property.Name);

                    var viewToViewModelAttribute = (ViewToViewModelAttribute)viewToViewModelAttributes[0];

                    string propertyName = property.Name;
                    string viewModelPropertyName = (string.IsNullOrEmpty(viewToViewModelAttribute.ViewModelPropertyName)) ? propertyName : viewToViewModelAttribute.ViewModelPropertyName;

                    var mapping = new ViewToViewModelMapping(propertyName, viewModelPropertyName, viewToViewModelAttribute.MappingType);

                    // Store it (in 2 dictionaries for high-speed access)
                    _viewToViewModelMappings.Add(property.Name, mapping);
                    _viewModelToViewMappings.Add(viewModelPropertyName, mapping);
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets all the <see cref="ViewToViewModelMapping"/> that are registered.
        /// </summary>
        /// <returns><see cref="IEnumerable{ViewToViewModelMapping}"/> containing all registered <see cref="ViewToViewModelMapping"/>.</returns>
        public IEnumerable<ViewToViewModelMapping> GetAllViewToViewModelMappings()
        {
            return _viewToViewModelMappings.Select(mapping => mapping.Value);
        }

        /// <summary>
        /// Determines whether the manager contains a view to view model property mapping for the specified view property name.
        /// </summary>
        /// <param name="viewPropertyName">Name of the view property.</param>
        /// <returns>
        /// <c>true</c> if the manager contains a view to view model property mapping for the specified view property name; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsViewToViewModelMapping(string viewPropertyName)
        {
            return _viewToViewModelMappings.ContainsKey(viewPropertyName);
        }

        /// <summary>
        /// Gets the <see cref="ViewToViewModelMapping"/> that is mapped to the specified view property name.
        /// </summary>
        /// <param name="viewPropertyName">Name of the view property.</param>
        /// <returns><see cref="ViewToViewModelMapping"/>.</returns>
        public ViewToViewModelMapping GetViewToViewModelMapping(string viewPropertyName)
        {
            return _viewToViewModelMappings[viewPropertyName];
        }

        /// <summary>
        /// Determines whether the manager contains a view model to view property mapping for the specified view model property name.
        /// </summary>
        /// <param name="viewModelPropertyName">Name of the view model property.</param>
        /// <returns>
        /// <c>true</c> if the manager contains a view model to view property mapping for the specified view model property name; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsViewModelToViewMapping(string viewModelPropertyName)
        {
            return _viewModelToViewMappings.ContainsKey(viewModelPropertyName);
        }

        /// <summary>
        /// Gets the <see cref="ViewToViewModelMapping"/> that is mapped to the specified view model property name.
        /// </summary>
        /// <param name="viewModelPropertyName">Name of the view model property.</param>
        /// <returns><see cref="ViewToViewModelMapping"/>.</returns>
        public ViewToViewModelMapping GetViewModelToViewMapping(string viewModelPropertyName)
        {
            return _viewModelToViewMappings[viewModelPropertyName];
        }
        #endregion
    }
}
