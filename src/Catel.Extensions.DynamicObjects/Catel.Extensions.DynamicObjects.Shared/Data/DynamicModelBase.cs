// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using Catel.Logging;
    using System.Collections.Generic;
    /// <summary>
    /// Dynamic model base implementing the <see cref="IDynamicMetaObjectProvider"/>.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class DynamicModelBase : ModelBase, IDynamicMetaObjectProvider
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Registers a simple property, which means only the name and type are required.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="PropertyData"/>.</returns>
        internal protected void RegisterDynamicProperty(string name, Type type)
        {
            var modelType = GetType();

            var propertyDataManager = PropertyDataManager;
            if (propertyDataManager.IsPropertyRegistered(modelType, name))
            {
                return;
            }

            Log.Debug("Registering dynamic property '{0}.{1}'", modelType.FullName, name);

            var propertyData = RegisterProperty(name, type);
            propertyData.IsDynamicProperty = true;

            InitializePropertyAfterConstruction(propertyData);
        }

        /// <summary>
        /// Gets the <see cref="IEnumerable{String}"/> containing all dynamic property names.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{String}"/> containing all dynamic property names.</returns>
        internal protected IEnumerable<string> GetDynamicPropertyNames()
        {
            return new List<string>(PropertyDataManager.GetDynamicPropertiesData(GetType()).Select(x => x.Name));
        }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject" /> responsible for binding operations performed on this object.
        /// </summary>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        /// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> to bind this object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            var metaObject = new DynamicModelBaseMetaObject(parameter, this);
            return metaObject;
        }

        /// <summary>
        /// Returns whether a specific property is registered as dynamic.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>True if the property is registered as dynamic, otherwise false.</returns>
        public bool IsPropertyRegisteredAsDynamic(string name)
        {
            return IsPropertyRegisteredAsDynamic(GetType(), name);
        }

        /// <summary>
        /// Returns whether a specific property is registered as dynamic.
        /// </summary>
        /// <typeparam name="T">Type of the object for which to check.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>
        /// True if the property is registered as dynamic, otherwise false.
        /// </returns>
        protected static bool IsPropertyRegisteredAsDynamic<T>(string name)
        {
            return IsPropertyRegistered(typeof(T), name);
        }

        /// <summary>
        /// Returns whether a specific property is registered as dynamic.
        /// </summary>
        /// <param name="type">The type of the object for which to check.</param>
        /// <param name="name">Name of the property.</param>
        /// <returns>
        /// True if the property is registered as dynamic, otherwise false.
        /// </returns>
        protected static bool IsPropertyRegisteredAsDynamic(Type type, string name)
        {
            return PropertyDataManager.IsPropertyRegisteredAsDynamic(type, name);
        }
    }
}