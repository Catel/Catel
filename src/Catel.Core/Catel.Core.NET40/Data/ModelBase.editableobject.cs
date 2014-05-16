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
#if NET
				[field: NonSerialized]
#endif
				private event EventHandler<EventArgs> _cancelEditingCompletedEvent;

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

				event EventHandler<EventArgs> IAdvancedEditableObject.CancelEditingCompleted
				{
					add
					{
						_cancelEditingCompletedEvent += value;
					}
					remove
					{
						_cancelEditingCompletedEvent += value;
					}
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
				/// <remarks>
				/// This event uses <see cref="EventArgs"/> instead of
				/// an derived version of <see cref="EditEventArgs"/> because
				/// having a Cancel flag would be misleading and there appears to
				/// be no need for the <see cref="EditEventArgs.EditableObject"/> as
				/// this object should be the same as what would have been put into 
				/// <see cref="EditEventArgs.EditableObject"/>.
				/// </remarks>
				protected virtual void OnCancelEditCompleted (EventArgs e)
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
					EventArgs	ea = new EventArgs();
            var eventArgs = new CancelEditEventArgs(this);
            _cancelEditingEvent.SafeInvoke(this, eventArgs);
            OnCancelEdit(eventArgs);

            if (eventArgs.Cancel)
            {
                Log.Info("IEditableObject.CancelEdit is canceled by the event args");
								goto cancel_completed;
						}

						if (_backup == null)
						{
							goto cancel_completed;
						}

						Log.Debug("IEditableObject.CancelEdit");

            _backup.RestoreBackup();
            _backup = null;

/*
 *			One could make the argument that the completed event should only
 *			occur if the cancel was allowed to complete.  However, I believe
 *			that consistency of the event should be the overriding factor.
 *			
 *			The user code can easily ignore the event when needed, and it is
 *			more difficult for the user code to synthesize the event if it was
 *			needed and not supplied.
 */
					cancel_completed:
						_cancelEditingCompletedEvent.SafeInvoke(this, ea);
						OnCancelEditCompleted(ea);
				}

        /// <summary>
        /// Pushes changes since the last <see cref="IEditableObject.BeginEdit()"/> call.
        /// </summary>
        void IEditableObject.EndEdit()
        {
            var eventArgs = new EndEditEventArgs(this);
            _endEditingEvent.SafeInvoke(this, eventArgs);
            OnEndEdit(eventArgs);

            if (eventArgs.Cancel)
            {
                Log.Info("IEditableObject.EndEdit is canceled by the event args");
                return;
            }

						if (_backup == null)
						{
							return;
						}

						Log.Debug("IEditableObject.EndEdit");

            _backup = null;
        }
    }
}