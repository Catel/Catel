// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.serialization.binary.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;
    using Catel.Runtime;
    using Catel.Scoping;
    using IoC;
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
            OnInitializing();

            Initialize();

            // Make sure this is not a first time call or custom call with null
            if (info == null)
            {
                FinishInitializationAfterConstructionOrDeserialization();
            }
            else
            {
                _serializationInfo = info;

                // Too bad we cannot put this in the BinarySerializer, but BinarySerialization works bottom => top. We
                // do need the GraphId though, thus we are setting it here
                var scopeName = string.Format("Thread_{0}", ThreadHelper.GetCurrentThreadId());
                using (var scopeManager = ScopeManager<ReferenceManager>.GetScopeManager(scopeName))
                {
                    var referenceManager = scopeManager.ScopeObject;

                    try
                    {
                        var graphId = (int)info.GetValue("GraphId", typeof (int));
                        referenceManager.RegisterManually(graphId, this);
                    }
                    catch (Exception)
                    {
                        // Swallow
                    }
                }
            }

            OnInitialized();
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
            var binarySerializer = SerializationFactory.GetBinarySerializer();
            var binarySerializationContext = new BinarySerializationContextInfo(info);

            binarySerializer.Serialize(this, binarySerializationContext);
        }

        /// <summary>
        /// Invoked when the deserialization of the object graph is complete.
        /// </summary>
        /// <param name="context">The <see cref="StreamingContext"/>..</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            IsDeserializedDataAvailable = true;

            var binarySerializer = SerializationFactory.GetBinarySerializer();
            var binarySerializationContext = new BinarySerializationContextInfo(_serializationInfo);

            binarySerializer.Deserialize(this, binarySerializationContext);

            DeserializationSucceeded = true;
        }
    }
}