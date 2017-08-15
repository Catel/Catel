// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.serialization.binary.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Catel.IoC;

#if NET || NETSTANDARD

namespace Catel.Data
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;
    using Catel.Runtime;
    using Catel.Scoping;
    using Runtime.Serialization;

    public partial class ModelBase
    {
        /// <summary>
        /// The <see cref="SerializationInfo"/> that is retrieved and will be used for deserialization.
        /// </summary>
        [field: NonSerialized]
        private readonly SerializationInfo _serializationInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// <para />
        /// Only constructor for the ModelBase.
        /// </summary>
        /// <param name="info">SerializationInfo object, null if this is the first time construction.</param>
        /// <param name="context">StreamingContext object, simple pass a default new StreamingContext() if this is the first time construction.</param>
        /// <remarks>
        /// Call this method, even when constructing the object for the first time (thus not deserializing).
        /// </remarks>
        protected ModelBase(SerializationInfo info, StreamingContext context)
        {
            Initialize();

            // Make sure this is not a first time call or custom call with null
            if (info != null)
            {
                _serializationInfo = info;

                // Too bad we cannot put this in the BinarySerializer, but BinarySerialization works bottom => top. We
                // do need the GraphId though, thus we are setting it here
                var scopeName = SerializationContextHelper.GetSerializationReferenceManagerScopeName();
                using (var scopeManager = ScopeManager<ReferenceManager>.GetScopeManager(scopeName))
                {
                    var referenceManager = scopeManager.ScopeObject;

                    int? graphId = null;

                    try
                    {
                        // Binary
                        graphId = (int)info.GetValue("GraphId", typeof (int));
                    }
                    catch (Exception)
                    {
                        // Swallow
                    }

                    if (graphId.HasValue)
                    {
                        referenceManager.RegisterManually(graphId.Value, this);
                    }
                }
            }
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var scopeName = SerializationContextHelper.GetSerializationReferenceManagerScopeName();
            using (var scopeManager = ScopeManager<SerializationScope>.GetScopeManager(scopeName, () => new SerializationScope(SerializationFactory.GetBinarySerializer(), null)))
            {
                var serializer = scopeManager.ScopeObject.Serializer;
                var configuration = scopeManager.ScopeObject.Configuration;

                var dependencyResolver = this.GetDependencyResolver();
                var serializationContextInfoFactory = dependencyResolver.Resolve<ISerializationContextInfoFactory>(serializer.GetType());

                var serializationContext = serializationContextInfoFactory.GetSerializationContextInfo(serializer, this, info, configuration);
                serializer.Serialize(this, serializationContext, configuration);
            }
        }

        /// <summary>
        /// Invoked when the deserialization of the object graph is complete.
        /// </summary>
        /// <param name="context">The <see cref="StreamingContext"/>..</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_serializationInfo == null)
            {
                // Probably a custom serializer which will populate us in a different way
                return;
            }

            var scopeName = SerializationContextHelper.GetSerializationReferenceManagerScopeName();
            using (var scopeManager = ScopeManager<SerializationScope>.GetScopeManager(scopeName, () => new SerializationScope(SerializationFactory.GetBinarySerializer(), null)))
            {
                var serializer = scopeManager.ScopeObject.Serializer;
                var configuration = scopeManager.ScopeObject.Configuration;

                var dependencyResolver = this.GetDependencyResolver();
                var serializationContextInfoFactory = dependencyResolver.Resolve<ISerializationContextInfoFactory>(serializer.GetType());

                var serializationContext = serializationContextInfoFactory.GetSerializationContextInfo(serializer, this, _serializationInfo, configuration);
                serializer.Deserialize(this, serializationContext, configuration);
            }
        }
    }
}

#endif