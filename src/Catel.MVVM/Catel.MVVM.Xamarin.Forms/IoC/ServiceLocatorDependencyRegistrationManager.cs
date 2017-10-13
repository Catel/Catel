// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorDependencyRegistrationManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Catel.Logging;
    using Catel.Reflection;
    using global::Xamarin.Forms;

    /// <summary>
    /// The service locator dependency registration manager class.
    /// </summary>
    public class ServiceLocatorDependencyRegistrationManager
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// An array of object with a single null element
        /// </summary>
        private static readonly object[] ArrayOfObjectWithSingleNullElement = { null };

        /// <summary>
        /// The dependency attribute Get method info.
        /// </summary>
        private static readonly MethodInfo DependencyServiceGetMethodInfo = typeof(DependencyService).GetMethodEx("Get", BindingFlags.Static | BindingFlags.Public);

        /// <summary>
        /// The dependency attribute Implementor property info.
        /// </summary>
        private static readonly PropertyInfo DependencyAttributeImplementorPropertyInfo = typeof(DependencyAttribute).GetPropertyEx("Implementor", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// The service locator.
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorDependencyRegistrationManager" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator</param>
        public ServiceLocatorDependencyRegistrationManager(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(()=> serviceLocator);

            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {
            //// TODO: Use type cache insted?
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainOnAssemblyLoad;
            foreach (var assembly in AppDomain.CurrentDomain.GetLoadedAssemblies())
            {
                LoadServiceFromAssembly(assembly);
            }
        }

        /// <summary>
        /// Loads dependency implementations.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        private void LoadServiceFromAssembly(Assembly assembly)
        {
            Log.Debug("Loading dependencies services into '{0}'...", assembly.FullName);

            foreach (var dependencyAttribute in assembly.GetCustomAttributes(typeof(DependencyAttribute)))
            {
                var serviceType = (Type)DependencyAttributeImplementorPropertyInfo.GetValue(dependencyAttribute);
                var interfaceType = serviceType.GetInterfacesEx().FirstOrDefault() ?? serviceType;

                Log.Debug("Found dependency service '{0}' and will be register as '{1}'", serviceType.FullName, interfaceType.FullName);

                _serviceLocator.RegisterType(interfaceType, serviceLocatorRegistration => DependencyServiceGetMethodInfo.MakeGenericMethod(interfaceType).Invoke(typeof(DependencyService), ArrayOfObjectWithSingleNullElement));
            }
        }

        /// <summary>
        /// Called when an assembly is loaded in the current <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AssemblyLoadEventArgs" /> instance containing the event data.</param>
        private void CurrentDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            LoadServiceFromAssembly(args.LoadedAssembly);
        }
    }
}