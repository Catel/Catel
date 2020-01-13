// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using Logging;

    using ISerializable = Catel.Runtime.Serialization.ISerializable;

#if !NET && !NETCORE && !NETSTANDARD
    using System.Reflection;
#endif

#if UWP
    using Windows.Storage.Streams;
#else
    using System.IO.IsolatedStorage;
#endif

    public partial class ModelBase
    {
        #region Methods
        /// <summary>
        /// Called when the object is being serialized.
        /// </summary>
        protected virtual void OnSerializing()
        {
        }

        /// <summary>
        /// Called when the object has been serialized.
        /// </summary>
        protected virtual void OnSerialized()
        {
            
        }

        /// <summary>
        /// Starts the serialization.
        /// </summary>
        void ISerializable.StartSerialization()
        {
            OnSerializing();
        }

        /// <summary>
        /// Finishes the serialization.
        /// </summary>
        void ISerializable.FinishSerialization()
        {
            OnSerialized();
        }

        /// <summary>
        /// Called when the object is being deserialized.
        /// </summary>
        protected virtual void OnDeserializing()
        {
        }

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        protected virtual void OnDeserialized()
        {
            IsDirty = false;
        }

        /// <summary>
        /// Begins the deserialization.
        /// </summary>
        void ISerializable.StartDeserialization()
        {
            //Log.Debug($"Start deserialization of '{GetType().Name}'");

            OnDeserializing();
        }

        /// <summary>
        /// Finishes the deserialization.
        /// </summary>
        void ISerializable.FinishDeserialization()
        {
            //Log.Debug($"Finish deserialization of '{GetType().Name}'");

            OnDeserialized();
        }
        #endregion
    }
}
