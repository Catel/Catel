// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.editableobject.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using IoC;
    using IO;
    using Logging;
    using Runtime.Serialization;

    public partial class ModelBase
    {
        internal ISerializer _editableObjectSerializer;

        #region Internal classes
        /// <summary>
        /// Class containing backup information.
        /// </summary>
        private class BackupData
        {
            #region Fields
            /// <summary>
            /// The <see cref="ModelBase"/> object that this backup is created for.
            /// </summary>
            private readonly ModelBase _object;

            /// <summary>
            /// The serializer used for this backup instance.
            /// </summary>
            private readonly ISerializer _serializer;

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
            /// Initializes a new instance of the <see cref="ModelBase.BackupData" /> class.
            /// </summary>
            /// <param name="obj">Object to backup.</param>
            /// <param name="serializer">The serializer.</param>
            public BackupData(ModelBase obj, ISerializer serializer)
            {
                _object = obj;
                _serializer = serializer;

                CreateBackup();
            }
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

                    _serializer?.SerializeMembers(_object, stream, null, propertiesToIgnore);

                    _propertyValuesBackup = stream.ToByteArray();
                }

                _objectValuesBackup = new Dictionary<string, object>();
                _objectValuesBackup.Add(nameof(IsDirty), BoxingCache.GetBoxedValue(_object.IsDirty));
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
                        var properties = new List<MemberValue>();

                        if (_serializer is not null)
                        {
                            properties = _serializer.DeserializeMembers(_object, stream, null);
                        }

                        oldPropertyValues = properties.ToDictionary(property => property.Name, property => property.Value);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to deserialize the data for backup");
                    }
                }

                if (oldPropertyValues is null)
                {
                    return;
                }

                foreach (var propertyValue in oldPropertyValues)
                {
                    if (PropertyDataManager.IsPropertyRegistered(_object.GetType(), propertyValue.Key))
                    {
                        // Set value so the PropertyChanged event is invoked
                        _object.SetValue(propertyValue.Key, propertyValue.Value);
                    }
                }

                _object.IsDirty = (bool)_objectValuesBackup[nameof(IsDirty)];
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// The backup of the current object if any backup is initiated.
        /// </summary>
        private BackupData _backup;

        private event EventHandler<BeginEditEventArgs> _beginEditingEvent;

        /// <summary>
        /// Occurs when the edit cancel has been completed or canceled.
        /// </summary>
        /// <remarks>
        /// This event uses <see cref="EventArgs"/> instead of
        /// an derived version of <see cref="EditEventArgs"/> because
        /// having a Cancel flag would be misleading and there appears to
        /// be no need for the <see cref="EditEventArgs.EditableObject"/> as
        /// the sender of the event should be the same information.
        /// </remarks>
        private event EventHandler<EventArgs> _cancelEditingCompletedEvent;

        private event EventHandler<CancelEditEventArgs> _cancelEditingEvent;

        private event EventHandler<EndEditEventArgs> _endEditingEvent;

        event EventHandler<BeginEditEventArgs> IAdvancedEditableObject.BeginEditing
        {
            add { _beginEditingEvent += value; }
            remove { _beginEditingEvent -= value; }
        }

        event EventHandler<EventArgs> IAdvancedEditableObject.CancelEditingCompleted
        {
            add { _cancelEditingCompletedEvent += value; }
            remove { _cancelEditingCompletedEvent += value; }
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
        /// Raises the <see cref="IAdvancedEditableObject.CancelEditingCompleted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnCancelEditCompleted(CancelEditCompletedEventArgs e)
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
        /// Gets the serializer for the <see cref="IEditableObject"/> interface implementation.
        /// </summary>
        /// <returns>The <see cref="ISerializer"/>.</returns>
        protected virtual ISerializer GetSerializerForIEditableObject()
        {
            if (!(_editableObjectSerializer is null))
            {
                return _editableObjectSerializer;
            }

            var dependencyResolver = this.GetDependencyResolver();

            var serializer = dependencyResolver.Resolve<ISerializer>();
            return serializer;
        }

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        void IEditableObject.BeginEdit()
        {
            if (_backup is not null)
            {
                Log.Debug("IEditableObject is already in edit state");
                return;
            }

            var eventArgs = new BeginEditEventArgs(this);
            _beginEditingEvent?.Invoke(this, eventArgs);
            OnBeginEdit(eventArgs);

            if (eventArgs.Cancel)
            {
                Log.Info("IEditableObject.BeginEdit is canceled by the event args");
                return;
            }

            Log.Debug("IEditableObject.BeginEdit");

            _backup = new BackupData(this, GetSerializerForIEditableObject());
        }

        /// <summary>
        /// Discards changes since the last <see cref="IEditableObject.BeginEdit()"/> call.
        /// </summary>
        void IEditableObject.CancelEdit()
        {
            if (_backup is null)
            {
                Log.Debug("IEditableObject is not in edit state");
                return;
            }

            CancelEditCompletedEventArgs cancelEditCompletedEventArgs;
            var eventArgs = new CancelEditEventArgs(this);
            _cancelEditingEvent?.Invoke(this, eventArgs);
            OnCancelEdit(eventArgs);

            if (eventArgs.Cancel)
            {
                Log.Info("IEditableObject.CancelEdit is canceled by the event args");
                cancelEditCompletedEventArgs = new CancelEditCompletedEventArgs(true);
                _cancelEditingCompletedEvent?.Invoke(this, cancelEditCompletedEventArgs);
                OnCancelEditCompleted(cancelEditCompletedEventArgs);
                return;
            }

            Log.Debug("IEditableObject.CancelEdit");

            _backup.RestoreBackup();
            _backup = null;

            cancelEditCompletedEventArgs = new CancelEditCompletedEventArgs(false);
            _cancelEditingCompletedEvent?.Invoke(this, cancelEditCompletedEventArgs);
            OnCancelEditCompleted(cancelEditCompletedEventArgs);
        }

        /// <summary>
        /// Pushes changes since the last <see cref="IEditableObject.BeginEdit()"/> call.
        /// </summary>
        void IEditableObject.EndEdit()
        {
            if (_backup is null)
            {
                Log.Debug("IEditableObject is not in edit state");
                return;
            }

            var eventArgs = new EndEditEventArgs(this);
            _endEditingEvent?.Invoke(this, eventArgs);
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
