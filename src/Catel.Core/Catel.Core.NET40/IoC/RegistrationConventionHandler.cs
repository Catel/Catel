// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationConventionHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
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
        private readonly IList<Assembly> _assemblies;

        /// <summary>
        /// The registered conventions
        /// </summary>
        private readonly IList<IRegistrationConvention> _registeredConventions = new List<IRegistrationConvention>();

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
        private IList<Type> _retrievedTypes;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationConventionHandler" /> class.
        /// </summary>
        public RegistrationConventionHandler(IServiceLocator serviceLocator = null, ITypeFactory typeFactory = null)
        {
            _serviceLocator = serviceLocator ?? this.GetServiceLocator();
            _typeFactory = typeFactory ?? this.GetTypeFactory();
            TypeFilter = new CompositeFilter<Type>();
            AssemblyFilter = new CompositeFilter<Assembly>();

            _assemblies = AssemblyHelper.GetLoadedAssemblies();

            AssemblyFilter.Excludes += assembly =>
            {
                var assemblyName = assembly.FullName;
                return !string.IsNullOrWhiteSpace(assemblyName) &&
                       (
#if !DEBUG
                           assemblyName.StartsWith("Catel") ||
#endif
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

            TypeFilter.Excludes += type => !string.IsNullOrWhiteSpace(type.Namespace) &&
                                           (
#if !DEBUG
                                               type.Namespace.StartsWith("Catel") || 
#endif
                                               type.Name.StartsWith("<")
                                               );
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
        /// <value>
        /// The registration conventions.
        /// </value>
        public IEnumerable<IRegistrationConvention> RegistrationConventions
        {
            get
            {
                lock (_registeredConventions)
                {
                    Log.Debug("Retrieving all registered registration conventions.");
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
            where TRegistrationConvention : IRegistrationConvention
        {
            var registrationConvention = _typeFactory.CreateInstanceWithParameters<TRegistrationConvention>(_serviceLocator, registrationType);

            if (_registeredConventions.Contains(registrationConvention))
            {
                return;
            }

            Log.Debug("Registering '{0}' on cached conventions", typeof (TRegistrationConvention).Name);

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

                var filterAssemblies = _assemblies.Where(assembly => AssemblyFilter.Matches(assembly));

                var types = filterAssemblies.SelectMany(TypeCache.GetTypesOfAssembly).Where(type => TypeFilter.Matches(type));

                _retrievedTypes = new List<Type>(types);

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
    }
}