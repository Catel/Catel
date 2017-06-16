// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Logging;

    using Runtime.Serialization;
    using Catel.Reflection;
    using ISerializable = Catel.Runtime.Serialization.ISerializable;

#if !NET
    using System.Reflection;
#endif

#if NET
    using System.Security;
#endif

#if NET
    using System.Runtime.Serialization.Formatters.Binary;
#elif NETFX_CORE
    using Windows.Storage.Streams;
#elif PCL
    // Not supported in Portable Class Library
#else
    using System.IO.IsolatedStorage;
#endif

    public partial class ModelBase
    {
        #region Fields
#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<EventArgs> _serialized;

#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<EventArgs> _deserialized;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the serializer used for internal model serialization (such as backups).
        /// </summary>
        /// <value>The serializer.</value>
        protected ISerializer Serializer { get; set; }

        /// <summary>
        /// Gets or sets the default serializer that will be used for the <see cref="Serializer"/> property.
        /// </summary>
        /// <value>The default serializer.</value>
        public static ISerializer DefaultSerializer { get; set; }

        /// <summary>
        /// Gets or sets the serialization configuration.
        /// </summary>
        /// <value>
        /// The serialization configuration.
        /// </value>
        protected ISerializationConfiguration SerializationConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the default serialization configuration.
        /// </summary>
        /// <value>
        /// The default serialization configuration.
        /// </value>
        public static ISerializationConfiguration DefaultSerializationConfiguration { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the object is serialized.
        /// </summary>
        event EventHandler<EventArgs> ISerializable.Serialized
        {
            add { _serialized += value; }
            remove { _serialized -= value; }
        }

        /// <summary>
        /// Occurs when the object is deserialized.
        /// </summary>
        event EventHandler<EventArgs> ISerializable.Deserialized
        {
            add { _deserialized += value; }
            remove { _deserialized -= value; }
        }
        #endregion

        #region Events
        #endregion

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
            _serialized.SafeInvoke(this);
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
            LeanAndMeanModel = true;
        }

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        protected virtual void OnDeserialized()
        {
            _deserialized.SafeInvoke(this);

            // Data is now considered deserialized
            IsDeserialized = true;

            FinishInitializationAfterConstructionOrDeserialization();

            IsDirty = false;

            LeanAndMeanModel = false;
        }

        /// <summary>
        /// Begins the deserialization.
        /// </summary>
        void ISerializable.StartDeserialization()
        {
            Log.Debug("Start deserialization of '{0}'", GetType().Name);

            OnDeserializing();
        }

        /// <summary>
        /// Finishes the deserialization.
        /// </summary>
        void ISerializable.FinishDeserialization()
        {
            Log.Debug("Finish deserialization of '{0}'", GetType().Name);

            OnDeserialized();
        }
        #endregion
    }
}