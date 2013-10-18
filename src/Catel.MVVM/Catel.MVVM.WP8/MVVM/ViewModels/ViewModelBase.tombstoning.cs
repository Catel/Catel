// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.tombstoning.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Catel.IoC;
    using Catel.Runtime.Serialization;
    using Phone.Controls;
    using Logging;

    /// <summary>
    /// Available tombstoning modes.
    /// </summary>
    public enum TombstoningMode
    {
        /// <summary>
        /// The view model will store and recover all values of all view model properties automatically.
        /// </summary>
        /// <remarks>
        /// This mode is not yet supported!
        /// </remarks>
        Auto,

        /// <summary>
        /// Tombstoning will be handled manually by the developer of the view models using the
        /// <see cref="ViewModelBase.PrepareForTombstoneState"/> and <see cref="ViewModelBase.RecoverFromTombstoneState"/>
        /// methods.
        /// </summary>
        Manual,

        /// <summary>
        /// Tombstoning capabilities are fully disabled for the view model.
        /// </summary>
        Disabled
    }

    public partial class ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the tombstoning mode.
        /// <para />
        /// The default value is <see cref="MVVM.TombstoningMode.Manual">TombstoningMode.Manual</see>.
        /// </summary>
        /// <value>The tombstoning mode.</value>
        public TombstoningMode TombstoningMode { get; protected set; }
        #endregion

        #region Events
        #endregion

        #region Methods
        /// <summary>
        /// Prepares the state for tombstoning.
        /// </summary>
        /// <param name="state">The target state which can be used to store values.</param>
        /// <remarks>
        /// This method is implemented so the <see cref="PhoneApplicationPage{TViewModel}"/> can call this method.
        /// </remarks>
        internal void PrepareForTombstoneStateInternal(IDictionary<string, object> state)
        {
            PrepareForTombstoneState(state);
        }

        /// <summary>
        /// Prepares the state for tombstoning.
        /// <para />
        /// This method will be called when the <see cref="TombstoningMode"/> is set to 
        /// <see cref="MVVM.TombstoningMode.Manual">TombstoningMode.Manual</see>.
        /// </summary>
        /// <param name="state">The target state which can be used to store values.</param>
        protected virtual void PrepareForTombstoneState(IDictionary<string, object> state)
        {
        }

        /// <summary>
        /// Recovers the state from tombstoning.
        /// </summary>
        /// <param name="state">The source state to recover values from.</param>
        /// <remarks>
        /// This method is implemented so the <see cref="PhoneApplicationPage{TViewModel}"/> can call this method.
        /// </remarks>
        internal void RecoverFromTombstoneStateInternal(IDictionary<string, object> state)
        {
            RecoverFromTombstoneState(state);
        }

        /// <summary>
        /// Recovers the state from tombstoning.
        /// <para />
        /// This method will be called when the <see cref="TombstoningMode"/> is set to
        /// <see cref="Catel.MVVM.TombstoningMode.Manual">TombstoningMode.Manual</see>.
        /// </summary>
        /// <param name="state">The source state to recover values from.</param>
        protected virtual void RecoverFromTombstoneState(IDictionary<string, object> state)
        {
        }

        /// <summary>
        /// Serializes the data in the view model for tombstoning.
        /// </summary>
        /// <returns>A byte array representing the data.</returns>
        internal byte[] SerializeForTombstoning()
        {
            using (var memoryStream = new MemoryStream())
            {
                var xmlSerializer = ServiceLocator.ResolveType<IXmlSerializer>();
                xmlSerializer.SerializeMembers(this, memoryStream);

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the data from a previously stored tombstoning state.
        /// </summary>
        /// <param name="data">The previously stored data.</param>
        internal void DeserializeFromTombstoning(byte[] data)
        {
            try
            {
                using (var memoryStream = new MemoryStream(data))
                {
                    var xmlSerializer = ServiceLocator.ResolveType<IXmlSerializer>();
                    var propertyValues = xmlSerializer.DeserializeMembers(GetType(), memoryStream);

                    LeanAndMeanModel = true;

                    foreach (var propertyValue in propertyValues)
                    {
                        SetValue(propertyValue.Name, propertyValue.Value, false, false);
                    }

                    LeanAndMeanModel = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deserializing the view model from tombstoned data");
            }
        }
        #endregion
    }
}