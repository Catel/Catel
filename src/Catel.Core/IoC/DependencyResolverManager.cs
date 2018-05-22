// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyResolverManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The dependency resolver manager.
    /// </summary>
    public class DependencyResolverManager : IDependencyResolverManager
    {
        #region Fields
        private IDependencyResolver _defaultDependencyResolver;

        private readonly object _lockObject = new object();

        private readonly ConditionalWeakTable<object, IDependencyResolver> _dependencyResolversByInstance = new ConditionalWeakTable<object, IDependencyResolver>(); 
        private readonly Dictionary<Type, IDependencyResolver> _dependencyResolversByType = new Dictionary<Type, IDependencyResolver>(); 
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="DependencyResolverManager"/> class.
        /// </summary>
        static DependencyResolverManager()
        {
            Default = new DependencyResolverManager();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyResolverManager"/> class.
        /// </summary>
        public DependencyResolverManager()
        {
            DefaultDependencyResolver = ServiceLocator.Default.ResolveType<IDependencyResolver>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or the default instance.
        /// </summary>
        /// <value>The default instance.</value>
        public static IDependencyResolverManager Default { get; set; }

        /// <summary>
        /// Gets or sets the default dependency resolver.
        /// </summary>
        /// <value>The default dependency resolver.</value>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <c>null</c>.</exception>
        public IDependencyResolver DefaultDependencyResolver
        {
            get
            {
                return _defaultDependencyResolver;
            }
            set
            {
                Argument.IsNotNull("value", value);

                _defaultDependencyResolver = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Registers the dependency resolver for a specific instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyResolver" /> is <c>null</c>.</exception>
        public virtual void RegisterDependencyResolverForInstance(object instance, IDependencyResolver dependencyResolver)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("dependencyResolver", dependencyResolver);

            lock (_lockObject)
            {
                _dependencyResolversByInstance.Add(instance, dependencyResolver);
            }
        }

        /// <summary>
        /// Gets the dependency resolver for a specific instance. If there is no dependency resolver registered for
        /// the specific instance, this method will use the <see cref="GetDependencyResolverForType" />.
        /// </summary>
        /// <param name="instance">The instance to retrieve the dependency resolver for.</param>
        /// <returns>The <see cref="IDependencyResolver" /> for the object.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        public virtual IDependencyResolver GetDependencyResolverForInstance(object instance)
        {
            Argument.IsNotNull("instance", instance);

            lock (_lockObject)
            {
                IDependencyResolver dependencyResolver = null;
                if (!_dependencyResolversByInstance.TryGetValue(instance, out dependencyResolver))
                {
                    dependencyResolver = GetDependencyResolverForType(instance.GetType());
                }

                return dependencyResolver;
            }
        }

        /// <summary>
        /// Registers the dependency resolver for a specific type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyResolver" /> is <c>null</c>.</exception>
        public virtual void RegisterDependencyResolverForType(Type type, IDependencyResolver dependencyResolver)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("dependencyResolver", dependencyResolver);

            lock (_lockObject)
            {
                _dependencyResolversByType[type] = dependencyResolver;
            }
        }

        /// <summary>
        /// Gets the dependency resolver for a specific type. If there is no dependency resolver registered for
        /// the specific type, this method will returns the <see cref="DefaultDependencyResolver" />.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="IDependencyResolver" /> for the type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public virtual IDependencyResolver GetDependencyResolverForType(Type type)
        {
            Argument.IsNotNull("type", type);

            lock (_lockObject)
            {
                if (_dependencyResolversByType.ContainsKey(type))
                {
                    return _dependencyResolversByType[type];
                }
            }

            return DefaultDependencyResolver;
        }
        #endregion
    }
}