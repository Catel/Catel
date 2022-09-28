// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorAutoRegistrationManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Logging;
    using Reflection;

    /// <summary>
    /// Manager that can handle the registrations of the service locator.
    /// </summary>
    public class ServiceLocatorAutoRegistrationManager
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// A single null element array of objects.
        /// </summary>
        private static readonly object[] SingleNullElementArrayOfObjects = new object[1]{ null };
        #endregion

        #region Fields
        /// <summary>
        /// The pending types.
        /// </summary>
        private readonly Queue<Type> _pendingTypes = new Queue<Type>();

        /// <summary>
        /// The service locator this manager is created for.
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// A value indicating whether this service locators will automatically register types via attributes.
        /// </summary>
        private bool _autoRegisterTypesViaAttributes;

        /// <summary>
        /// Indicates whether the service locator has inspected the types at least once.
        /// </summary>
        private bool _hasInspectedTypesAtLeastOnce;

        /// <summary>
        /// Indicates whether the loaded types are inspecting by this service locator. 
        /// </summary>
        private bool _isInspectedTypes;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorAutoRegistrationManager" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        public ServiceLocatorAutoRegistrationManager(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            _serviceLocator = serviceLocator;

            if (EnvironmentHelper.IsProcessCurrentlyHostedByTool())
            {
                return;
            }

            TypeCache.AssemblyLoaded += (sender, args) =>
            {
                if (_autoRegisterTypesViaAttributes)
                {
                    if (_hasInspectedTypesAtLeastOnce)
                    {
                        // Only required if assemblies are lazy-loaded
                        foreach (var type in args.LoadedTypes)
                        {
                            _pendingTypes.Enqueue(type);
                        }
                    }

                    try
                    {
                        InspectLoadedAssemblies();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to handle dynamically loaded assembly '{0}'", args.Assembly.FullName);
                    }
                }
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this service locators will ignore incorrect usage of <see cref="ServiceLocatorRegistrationAttribute"/> 
        /// and do not throw <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <remarks>
        /// By default, this value is <c>true</c>.
        /// </remarks>
        public bool IgnoreRuntimeIncorrectUsageOfRegisterAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this service locators will automatically register types via attributes.
        /// </summary>
        /// <remarks>
        /// By default, this value is <c>false</c>.
        /// </remarks>
        public bool AutoRegisterTypesViaAttributes
        {
            get
            {
                return _autoRegisterTypesViaAttributes;
            }
            set
            {
                if (_autoRegisterTypesViaAttributes != value)
                {
                    _autoRegisterTypesViaAttributes = value;

                    if (_autoRegisterTypesViaAttributes)
                    {
                        if (!_hasInspectedTypesAtLeastOnce)
                        {
                            foreach (var type in TypeCache.GetTypes(allowInitialization: true))
                            {
                                _pendingTypes.Enqueue(type);
                            }

                            _hasInspectedTypesAtLeastOnce = true;
                        }

                        InspectLoadedAssemblies();
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Inspect loaded assemblies.
        /// </summary>
        private void InspectLoadedAssemblies()
        {
            if (!_isInspectedTypes)
            {
                _isInspectedTypes = true;

                try
                {
                    var typeFactory = _serviceLocator.ResolveType<ITypeFactory>();

                    while (_pendingTypes.Count > 0)
                    {
                        var type = _pendingTypes.Dequeue();

                        if (type.TryGetAttribute(out ServiceLocatorRegistrationAttribute attribute))
                        {
                            // CTL-780: support open generics
                            if (!type.IsGenericTypeEx() && (type.IsAbstractEx() || !attribute.InterfaceType.IsAssignableFromEx(type)))
                            {
                                var message = string.Format("The type '{0}' is abstract or can't be registered as '{1}'", type, attribute.InterfaceType);

                                if (!IgnoreRuntimeIncorrectUsageOfRegisterAttribute)
                                {
                                    throw Log.ErrorAndCreateException<InvalidOperationException>(message);
                                }

                                Log.Warning(message);
                            }
                            else
                            {
                                switch (attribute.RegistrationMode)
                                {
                                    case ServiceLocatorRegistrationMode.Transient:
                                    case ServiceLocatorRegistrationMode.SingletonInstantiateWhenRequired:
                                        _serviceLocator.RegisterTypeIfNotYetRegisteredWithTag(attribute.InterfaceType, type, attribute.Tag, attribute.RegistrationType);
                                        break;

                                    case ServiceLocatorRegistrationMode.SingletonInstantiateImmediately:
                                        if (!_serviceLocator.IsTypeRegistered(attribute.InterfaceType))
                                        {
                                            var instance = typeFactory.CreateInstance(type);
                                            if (instance is null)
                                            {
                                                Log.Error("Failed to instantiate type '{0}', cannot automatically register the instance", type.GetSafeFullName(false));
                                            }
                                            else
                                            {
                                                _serviceLocator.RegisterInstance(attribute.InterfaceType, instance, attribute.Tag);
                                            }
                                        }
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    AutoRegisterTypesViaAttributes = false;

                    throw Log.ErrorAndCreateException<InvalidOperationException>(ex, "Failed to inspect the pending types");
                }
                finally
                {
                    _isInspectedTypes = false;
                }
            }
        }
        #endregion
    }
}
