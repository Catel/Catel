// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationConventionHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Catel.Data;
    using Collections;
    using Logging;
    using Reflection;

    /// <summary>
    /// Represents the <see cref="IRegistrationConventionHandler"/> implementation.
    /// </summary>
    public class RegistrationConventionHandler : IRegistrationConventionHandler
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The static instance of the registration convention handler.
        /// </summary>
        private static readonly IRegistrationConventionHandler _default = new RegistrationConventionHandler();
        #endregion

        #region Fields
        /// <summary>
        /// The assemblies
        /// </summary>
        private readonly HashSet<Assembly> _assemblies;

        /// <summary>
        /// The registered conventions
        /// </summary>
        private readonly HashSet<IRegistrationConvention> _registeredConventions = new HashSet<IRegistrationConvention>();

        /// <summary>
        /// The service locator
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// The type factory
        /// </summary>
        private readonly ITypeFactory _typeFactory;

        /// <summary>
        /// The retrieved types
        /// </summary>
        private HashSet<Type> _retrievedTypes;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationConventionHandler" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="typeFactory">The type factory.</param>
        public RegistrationConventionHandler(IServiceLocator serviceLocator = null, ITypeFactory typeFactory = null)
        {
            _serviceLocator = serviceLocator ?? this.GetServiceLocator();
            _typeFactory = typeFactory ?? this.GetTypeFactory();

            TypeFilter = new CompositeFilter<Type>();
            AssemblyFilter = new CompositeFilter<Assembly>();

            _assemblies = new HashSet<Assembly>(AssemblyHelper.GetLoadedAssemblies());

            AssemblyFilter.Excludes += assembly =>
            {
                var assemblyName = assembly.FullName;
                return !string.IsNullOrWhiteSpace(assemblyName) && (
                           assemblyName.StartsWith("System") ||
                           assemblyName.StartsWith("WindowsBase") ||
                           assemblyName.StartsWith("Microsoft") ||
                           assemblyName.StartsWith("Presentation") ||
                           assemblyName.StartsWith("NuGet") ||
                           assemblyName.StartsWith("EntityFramework") ||
                           assemblyName.StartsWith("FluentValidation") ||
                           assemblyName.StartsWith("mscorlib") ||
                           assemblyName.StartsWith("UIAutomationProvider") ||
                           assemblyName.StartsWith("Anonymously") ||
                           assemblyName.StartsWith("SMDiagnostics")
                           );
            };

            TypeFilter.Excludes += type => !type.IsInterfaceEx() && !type.GetParentTypes().Any(parentType => parentType.IsInterfaceEx());
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default instance of the registration convention handler.
        /// </summary>
        /// <value>The default instance.</value>
        public static IRegistrationConventionHandler Default
        {
            get { return _default; }
        }
        #endregion

        #region IRegistrationConventionHandler Members
        /// <summary>
        /// Gets the registration conventions.
        /// </summary>
        /// <value>The registration conventions.</value>
        public IEnumerable<IRegistrationConvention> RegistrationConventions
        {
            get
            {
                lock (_registeredConventions)
                {
                    return _registeredConventions;
                }
            }
        }

        /// <summary>
        /// Gets the type filter.
        /// </summary>
        /// <value>The type filter.</value>
        public ICompositeFilter<Type> TypeFilter { get; private set; }

        /// <summary>
        /// Gets the assembly filter.
        /// </summary>
        /// <value>The assembly filter.</value>
        public ICompositeFilter<Assembly> AssemblyFilter { get; private set; }

        /// <summary>
        /// Registers the convention.
        /// </summary>
        public void RegisterConvention<TRegistrationConvention>(RegistrationType registrationType = RegistrationType.Singleton)
            where TRegistrationConvention : class, IRegistrationConvention
        {
#pragma warning disable HAA0101 // Array allocation for params parameter
            var registrationConvention = _typeFactory.CreateInstanceWithParameters<TRegistrationConvention>(_serviceLocator, BoxingCache<RegistrationType>.Default.GetBoxedValue(registrationType));
#pragma warning restore HAA0101 // Array allocation for params parameter

            if (_registeredConventions.Contains(registrationConvention))
            {
                return;
            }

            Log.Debug("Registering '{0}' on cached conventions", typeof(TRegistrationConvention).Name);

            _registeredConventions.Add(registrationConvention);

            ApplyConventions();
        }

        /// <summary>
        /// Applies the registered conventions.
        /// </summary>
        public void ApplyConventions()
        {
            lock (_registeredConventions)
            {
                if (!_registeredConventions.Any())
                {
                    return;
                }

                var types = new List<Type>();

                var filteredAssemblies = _assemblies.Where(assembly => AssemblyFilter.Matches(assembly));
                foreach (var filteredAssembly in filteredAssemblies)
                {
                    types.AddRange(TypeCache.GetTypesOfAssembly(filteredAssembly, type => !string.IsNullOrWhiteSpace(type.Namespace) && !type.Name.StartsWith("<")));
                }

                types.ForEach(RemoveIfAlreadyRegistered);

                var typesToHandle = types.Where(type => TypeFilter.Matches(type));

                _retrievedTypes = new HashSet<Type>(typesToHandle);

                if (!_retrievedTypes.Any())
                {
                    return;
                }

                _registeredConventions.ForEach(convention => convention.Process(_retrievedTypes));
            }
        }

        /// <summary>
        /// Adds the assembly to scan.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="assembly" /> is <c>null</c>.</exception>
        public void AddAssemblyToScan(Assembly assembly)
        {
            Argument.IsNotNull("assembly", assembly);

            lock (_assemblies)
            {
                if (_assemblies.Contains(assembly))
                {
                    return;
                }

                _assemblies.Add(assembly);

                ApplyConventions();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Removes the specified type in the container if already registered.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        protected void RemoveIfAlreadyRegistered(Type type)
        {
            Argument.IsNotNull("type", type);

            if (_serviceLocator.IsTypeRegistered(type) && !type.IsAssignableFromEx(typeof(IServiceLocator)) && !type.IsAssignableFromEx(typeof(ITypeFactory)) && !type.IsAssignableFromEx(typeof(IDependencyResolver)) && !type.IsAssignableFromEx(typeof(IRegistrationConventionHandler)))
            {
                _serviceLocator.RemoveType(type);
            }
        }
        #endregion
    }
}
