namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Catel.Reflection;
    using Logging;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Property data manager.
    /// </summary>
    public class PropertyDataManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(PropertyDataManager));

        /// <summary>
        /// Dictionary containing all the properties per type.
        /// </summary>
        private readonly Dictionary<Type, CatelTypeInfo> _propertyData = new Dictionary<Type, CatelTypeInfo>();

        /// <summary>
        /// Lock object for the <see cref="_propertyData"/> field.
        /// </summary>
        private readonly object _propertyDataLock = new object();

        /// <summary>
        /// Initializes static members of the <see cref="PropertyDataManager" /> class.
        /// </summary>
        static PropertyDataManager()
        {
            Default = new PropertyDataManager();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDataManager"/> class.
        /// </summary>
        public PropertyDataManager()
        {
        }

        /// <summary>
        /// Gets the default instance of the property data manager.
        /// </summary>
        /// <value>The default.</value>
        public static PropertyDataManager Default { get; private set; }

        /// <summary>
        /// Gets the property data type information.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="CatelTypeInfo"/> representing the specified type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public CatelTypeInfo GetCatelTypeInfo(Type type)
        {
            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var typeInfo))
                {
                    typeInfo = RegisterProperties(type);
                }

                return typeInfo;
            }
        }

        /// <summary>
        /// Registers all the properties for the specified type.
        /// <para />
        /// This method can only be called once per type. The <see cref="PropertyDataManager"/> caches
        /// whether it has already registered the properties once.
        /// </summary>
        /// <param name="type">The type to register the properties for.</param>
        /// <returns>The property data type info.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The properties are not declared correctly.</exception>
        public CatelTypeInfo RegisterProperties(Type type)
        {
            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var typeInfo))
                {
                    typeInfo = new CatelTypeInfo(type);
                    _propertyData[type] = typeInfo;
                }

                return typeInfo;
            }
        }

        /// <summary>
        /// Registers a property for a specific type.
        /// </summary>
        /// <param name="type">The type for which to register the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyData">The property data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">A property with the same name is already registered.</exception>
        public void RegisterProperty(Type type, string name, IPropertyData propertyData)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var typeInfo))
                {
                    typeInfo = new CatelTypeInfo(type);
                    _propertyData[type] = typeInfo;
                }

                typeInfo.RegisterProperty(name, propertyData);
            }
        }

        /// <summary>
        /// Unregisters a property for a specific type.
        /// </summary>
        /// <param name="type">The type for which to register the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public void UnregisterProperty(Type type, string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var typeInfo))
                {
                    typeInfo = new CatelTypeInfo(type);
                    _propertyData[type] = typeInfo;
                }

                typeInfo.UnregisterProperty(name);
            }
        }

        /// <summary>
        /// Returns whether a specific property is registered.
        /// </summary>
        /// <param name="type">The type for which to check whether the property is registered.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        /// True if the property is registered, otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public bool IsPropertyRegistered(Type type, string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var propertyDataOfType))
                {
                    return false;
                }

                return propertyDataOfType.IsPropertyRegistered(name);
            }
        }

        /// <summary>
        /// Gets the property data.
        /// </summary>
        /// <param name="type">The type for which to get the property data.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The <see cref="PropertyData"/> of the requested property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        public IPropertyData GetPropertyData(Type type, string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var propertyDataOfType))
                {
                    throw Logger.LogErrorAndCreateException(msg => new PropertyNotRegisteredException(name, type),
                        "Property '{0}' on type '{1}' is not registered", name, type.GetSafeFullName());
                }

                return propertyDataOfType.GetPropertyData(name);
            }
        }

        /// <summary>
        /// Gets the property data.
        /// </summary>
        /// <param name="type">The type for which to get the property data.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyData">The <see cref="PropertyData"/> of the requested property or <c>null</c> if the property cannot be found.</param>
        /// <returns><c>true</c> if the property is returned, otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        public bool TryGetPropertyData(Type type, string name, [NotNullWhen(true)]out IPropertyData? propertyData)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var propertyDataOfType))
                {
                    propertyData = null;
                    return false;
                }

                return propertyDataOfType.TryGetPropertyData(name, out propertyData);
            }
        }
    }
}
