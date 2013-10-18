// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Dynamic;
    using System.Linq.Expressions;
    using Catel.Logging;

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
        internal void RegisterDynamicProperty(string name, Type type)
        {
            var modelType = GetType();

            var propertyDataManager = PropertyDataManager;
            if (propertyDataManager.IsPropertyRegistered(modelType, name))
            {
                return;
            }

            Log.Debug("Registering dynamic property '{0}.{1}'", modelType.FullName, name);

            var propertyData = RegisterProperty(name, type);

            InitializePropertyAfterConstruction(propertyData);
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
    }
}