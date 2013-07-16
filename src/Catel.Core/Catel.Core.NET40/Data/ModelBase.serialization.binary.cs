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
            binarySerializer.Serialize(this, info);
        }

        /// <summary>
        /// Invoked when the deserialization of the object graph is complete.
        /// </summary>
        /// <param name="context">The <see cref="StreamingContext"/>..</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            //Log.Debug("Received OnDeserialized event for '{0}'", GetType().Name);

            IsDeserializedDataAvailable = true;

            var binarySerializer = SerializationFactory.GetBinarySerializer();
            binarySerializer.Deserialize(this, _serializationInfo);

            DeserializationSucceeded = true;
        }
    }
}