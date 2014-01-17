// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.editableobject.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using IO;
    using Logging;
    using Runtime.Serialization;

#if !NET
    using System.Runtime.Serialization;
#endif

    public partial class ModelBase
    {
        #region Internal classes
        /// <summary>
        /// Class containing backup information.
        /// </summary>
#if !NET
        [DataContract]
#endif
        private class BackupData
        {
            #region Constants

            /// <summary>
            /// The name of the <see cref="ModelBase.IsDirty"/> property.
            /// </summary>
            private const string IsDirty = "IsDirty";

            #endregion

            #region Fields

            /// <summary>
            /// The <see cref="ModelBase"/> object that this backup is created for.
            /// </summary>
            private readonly ModelBase _object;

            /// <summary>
            /// Backup of the property values.
            /// </summary>
            private byte[] _propertyValuesBackup;

            /// <summary>
            /// Backup of the object values.
            /// </summary>
            private Dictionary<string, object> _objectValuesBackup;
            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ModelBase.BackupData"/> class.
            /// </summary>
            /// <param name="obj">Object to backup.</param>
            public BackupData(ModelBase obj)
            {
                Argument.IsNotNull("obj", obj);

                _object = obj;

                CreateBackup();
            }

            #endregion

            #region Properties

            #endregion

            #region Methods

            /// <summary>
            /// Creates a backup of the object property values.
            /// </summary>
            private void CreateBackup()
            {
                using (var stream = new MemoryStream())
                {
                    var catelTypeInfo = PropertyDataManager.GetCatelTypeInfo(_object.GetType());
                    var propertiesToIgnore = (from propertyData in catelTypeInfo.GetCatelProperties()
                                              where !propertyData.Value.IncludeInBackup
                                              select propertyData.Value.Name).ToArray();

                    var serializer = SerializationFactory.GetXmlSerializer();
                    serializer.SerializeMembers(_object, stream, propertiesToIgnore);

                    _propertyValuesBackup = stream.ToByteArray();
                }

                _objectValuesBackup = new Dictionary<string, object>();
                _objectValuesBackup.Add(IsDirty, _object.IsDirty);
            }

            /// <summary>
            /// Restores the backup to the object.
            /// </summary>
            public void RestoreBackup()
            {
                Dictionary<string, object> oldPropertyValues = null;

                using (var stream = new MemoryStream(_propertyValuesBackup))
                {
                    try
                    {
                        var serializer = SerializationFactory.GetXmlSerializer();
                        var properties = serializer.DeserializeMembers(_object.GetType(), stream);

                        oldPropertyValues = properties.ToDictionary(property => property.Name, property => property.Value);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to deserialize the data for backup, which is weird. However, for Silverlight, Windows Phone and Windows 8 there is no other option");
                    }
                }

                if (oldPropertyValues == null)
                {
                    return;
                }

                foreach (KeyValuePair<string, object> propertyValue in oldPropertyValues)
                {
                    if (PropertyDataManager.IsPropertyRegistered(_object.GetType(), propertyValue.Key))
                    {
                        // Set value so the PropertyChanged event is invoked
                        _object.SetValue(propertyValue.Key, propertyValue.Value);
                    }
                }

                _object.IsDirty = (bool)_objectValuesBackup[IsDirty];
            }

            #endregion
        }
        #endregion

        /// <summary>
        /// The backup of the current object if any backup is initiated.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private BackupData _backup;

#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<BeginEditEventArgs> _beginEditingEvent;

#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<CancelEditEventArgs> _cancelEditingEvent;

#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<EndEditEventArgs> _endEditingEvent;

        event EventHandler<BeginEditEventArgs> IAdvancedEditableObject.BeginEditing
        {
            add { _beginEditingEvent += value; }
            remove { _beginEditingEvent -= value; }
        }

        event EventHandler<CancelEditEventArgs> IAdvancedEditableObject.CancelEditing
        {
            add { _cancelEditingEvent += value; }
            remove { _cancelEditingEvent += value; }
        }

        event EventHandler<EndEditEventArgs> IAdvancedEditableObject.EndEditing
        {
            add { _endEditingEvent += value; }
            remove { _endEditingEvent += value; }
        }

        /// <summary>
        /// Raises the <see cref="IEditableObject.BeginEdit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.BeginEditEventArgs"/> instance containing the event data.</param>
        protected virtual void OnBeginEdit(BeginEditEventArgs e)
        {
        }

        /// <summary>
        /// Raises the <see cref="IEditableObject.CancelEdit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.EditEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCancelEdit(EditEventArgs e)
        {
        }

        /// <summary>
        /// Raises the <see cref="IEditableObject.EndEdit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.EditEventArgs"/> instance containing the event data.</param>
        protected virtual void OnEndEdit(EditEventArgs e)
        {
        }

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        void IEditableObject.BeginEdit()
        {
            var eventArgs = new BeginEditEventArgs(this);
            _beginEditingEvent.SafeInvoke(this, eventArgs);
            OnBeginEdit(eventArgs);

            if (_backup != null)
            {
                return;
            }

            if (eventArgs.Cancel)
            {
                Log.Info("IEditableObject.BeginEdit is canceled by the event args");
                return;
            }

            Log.Debug("IEditableObject.BeginEdit");

            _backup = new BackupData(this);
        }

        /// <summary>
        /// Discards changes since the last <see cref="IEditableObject.BeginEdit()"/> call.
        /// </summary>
        void IEditableObject.CancelEdit()
        {
            if (_backup == null)
            {
                return;
            }

            var eventArgs = new CancelEditEventArgs(this);
            _cancelEditingEvent.SafeInvoke(this, eventArgs);
            OnCancelEdit(eventArgs);

            if (eventArgs.Cancel)
            {
                Log.Info("IEditableObject.CancelEdit is canceled by the event args");
                return;
            }

            Log.Debug("IEditableObject.CancelEdit");

            _backup.RestoreBackup();
            _backup = null;
        }

        /// <summary>
        /// Pushes changes since the last <see cref="IEditableObject.BeginEdit()"/> call.
        /// </summary>
        void IEditableObject.EndEdit()
        {
            if (_backup == null)
            {
                return;
            }

            var eventArgs = new EndEditEventArgs(this);
            _endEditingEvent.SafeInvoke(this, eventArgs);
            OnEndEdit(eventArgs);

            if (eventArgs.Cancel)
            {
                Log.Info("IEditableObject.EndEdit is canceled by the event args");
                return;
            }

            Log.Debug("IEditableObject.EndEdit");

            _backup = null;
        }
    }
}