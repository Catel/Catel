// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MementoService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Memento
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using Collections;
    using Logging;

    /// <summary>
    /// The memento service allows the usage of the memento pattern. This means that this service provides possibilities
    /// to undo/redo all steps taken in an application.
    /// </summary>
    public class MementoService : IMementoService
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default maximum supported actions.
        /// </summary>
        public const int DefaultMaximumSupportedActions = 300;

        private readonly object _lock = new object();
        private int _maximumSupportedOperations;
        private bool _isUndoingOperation;

        private readonly List<IMementoBatch> _undoBatches = new List<IMementoBatch>();
        private readonly List<IMementoBatch> _redoBatches = new List<IMementoBatch>();
        private Batch _currentBatch;

        private readonly Dictionary<object, ObserverBase> _observers = new Dictionary<object, ObserverBase>();

        /// <summary>
        /// The static instance of the memento service.
        /// </summary>
        private static readonly IMementoService _instance = new MementoService();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class with the default number
        /// of supported undo and redo actions which is <see cref="DefaultMaximumSupportedActions"/>.
        /// </summary>
        public MementoService()
            : this(DefaultMaximumSupportedActions) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MementoService"/> class.
        /// </summary>
        /// <param name="maximumSupportedBatches">The max supported undo and redo actions.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="maximumSupportedBatches"/> is smaller than <c>0</c>.</exception>
        public MementoService(int maximumSupportedBatches)
        {
            Argument.IsMinimal("maximumSupportedBatches", maximumSupportedBatches, 0);

            MaximumSupportedBatches = maximumSupportedBatches;
            IsEnabled = true;

            Log.Debug("Initialized MementoService with {0} supported batches", maximumSupportedBatches);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default instance of the memento service.
        /// </summary>
        /// <value>The default instance.</value>
        public static IMementoService Default
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets or sets the maximum number of supported batches.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value"/> is smaller than <c>0</c>.</exception>
        public int MaximumSupportedBatches
        {
            get
            {
                return _maximumSupportedOperations;
            }
            set
            {
                Argument.IsMinimal("value", value, 0);

                if (_maximumSupportedOperations == value)
                {
                    return;
                }

                _maximumSupportedOperations = value;

                lock (_lock)
                {
                    while (_undoBatches.Count > _maximumSupportedOperations)
                    {
                        _undoBatches.RemoveLast();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the service is enabled.
        /// </summary>
        /// <value><c>true</c> if the service is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is at least one Undo operation we can perform.
        /// </summary>
        /// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
        public bool CanUndo
        {
            get
            {
                lock (_lock)
                {
                    return !_isUndoingOperation && _undoBatches.Count > 0;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is at least one Redo operation we can perform.
        /// </summary>
        /// <value><c>true</c> if this instance can redo; otherwise, <c>false</c>.</value>
        public bool CanRedo
        {
            get
            {
                lock (_lock)
                {
                    return !_isUndoingOperation && _redoBatches.Count > 0;
                }
            }
        }

        /// <summary>
        /// Gets the redo batches.
        /// </summary>
        /// <value>The redo batches.</value>
        public IEnumerable<IMementoBatch> RedoBatches
        {
            get
            {
                lock (_lock)
                {
                    return _redoBatches;
                }
            }
        }

        /// <summary>
        /// Gets the undo batches.
        /// </summary>
        /// <value>The undo batches.</value>
        public IEnumerable<IMementoBatch> UndoBatches
        {
            get
            {
                lock (_lock)
                {
                    return _undoBatches;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Begins a new batch. 
        /// <para />
        /// Note that this method will always call <see cref="EndBatch"/> before creating the new batch to ensure
        /// that a new batch is actually created.
        /// <para />
        /// All operations added via the <see cref="Add(Catel.Memento.IMementoSupport,bool)"/> will belong the this batch
        /// and be handled as a single operation.
        /// </summary>
        /// <param name="title">The title which can be used to display this batch i a user interface.</param>
        /// <param name="description">The description which can be used to display this batch i a user interface.</param>
        /// <returns>The <see cref="IMementoBatch" /> that has just been created.</returns>
        public IMementoBatch BeginBatch(string title = null, string description = null)
        {
            EndBatch();

            var batch = new Batch
                {
                    Title = title,
                    Description = description
                };

            Log.Debug("Starting batch with title '{0}' and description '{1}'", ObjectToStringHelper.ToString(batch.Title),
                ObjectToStringHelper.ToString(batch.Description));

            _currentBatch = batch;

            return batch;
        }

        /// <summary>
        /// Ends the current batch and adds it to the stack by calling <see cref="Add(Catel.Memento.IMementoBatch,bool)"/>.
        /// <para />
        /// If there is currently no batch, this method will silently exit.
        /// </summary>
        /// <returns>The <see cref="IMementoBatch"/> that has just been ended or <c>null</c> if there was no current batch.</returns>
        public IMementoBatch EndBatch()
        {
            if (_currentBatch == null)
            {
                return null;
            }

            var batch = _currentBatch;

            Add(batch);

            _currentBatch = null;

            Log.Debug("Ended batch with title '{0}' and description '{1}' with '{2}' actions", ObjectToStringHelper.ToString(batch.Title),
                ObjectToStringHelper.ToString(batch.Description), batch.ActionCount);

            return batch;
        }

        /// <summary>
        /// Executes the next undo operation.
        /// </summary>
        /// <returns><c>true</c> if an undo was executed; otherwise <c>false</c>.</returns>
        public bool Undo()
        {
            if (CanUndo)
            {
                IMementoBatch undo = null;

                lock (_lock)
                {
                    if (_undoBatches.Count > 0)
                    {
                        _isUndoingOperation = true;

                        undo = _undoBatches.First();
                        _undoBatches.RemoveFirst();
                    }
                }

                if (undo != null)
                {
                    try
                    {
                        undo.Undo();
                        if (undo.CanRedo)
                        {
                            lock (_lock)
                            {
                                _redoBatches.Insert(0, undo);
                                while (_redoBatches.Count > _maximumSupportedOperations)
                                {
                                    _redoBatches.RemoveLast();
                                }
                            }
                        }

                        return true;
                    }
                    finally
                    {
                        lock (_lock)
                        {
                            _isUndoingOperation = false;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Executes the last redo operation.
        /// </summary>
        /// <returns><c>true</c> if a redo operation occurred; otherwise <c>false</c>.</returns>
        public bool Redo()
        {
            if (CanRedo)
            {
                IMementoBatch undo = null;

                lock (_lock)
                {
                    if (_redoBatches.Count > 0)
                    {
                        _isUndoingOperation = true;
                        undo = _redoBatches.First();
                        _redoBatches.RemoveFirst();
                    }
                }

                if (undo != null && undo.CanRedo)
                {
                    try
                    {
                        undo.Redo();
                        lock (_lock)
                        {
                            _undoBatches.Insert(0, undo);
                            while (_undoBatches.Count > _maximumSupportedOperations)
                            {
                                _undoBatches.RemoveLast();
                            }
                        }

                        return true;
                    }
                    finally
                    {
                        lock (_lock)
                        {
                            _isUndoingOperation = false;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a new undo operation to the stack.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="noInsertIfExecutingOperation">Do not insert record if currently running undo/redo.</param>
        /// <returns><c>true</c> if undo operation was added to stack; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="operation"/> is <c>null</c>.</exception>
        public bool Add(IMementoSupport operation, bool noInsertIfExecutingOperation = true)
        {
            Argument.IsNotNull("operation", operation);

            if (!IsEnabled)
            {
                return false;
            }

            if (noInsertIfExecutingOperation && _isUndoingOperation)
            {
                return false;
            }

            lock (_lock)
            {
                if (_currentBatch != null)
                {
                    _currentBatch.AddAction(operation);
                }
                else
                {
                    var batch = new Batch();
                    batch.AddAction(operation);

                    Add(batch);
                }
            }

            return true;
        }

        /// <summary>
        /// Adds a new batch to the stack.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="noInsertIfExecutingOperation">Do not insert record if currently running undo/redo.</param>
        /// <returns><c>true</c> if record inserted; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="batch"/> is <c>null</c>.</exception>
        public bool Add(IMementoBatch batch, bool noInsertIfExecutingOperation = true)
        {
            Argument.IsNotNull("batch", batch);

            if (!IsEnabled)
            {
                return false;
            }

            if (noInsertIfExecutingOperation && _isUndoingOperation)
            {
                return false;
            }

            lock (_lock)
            {
                _undoBatches.Insert(0, batch);

                while (_undoBatches.Count > MaximumSupportedBatches)
                {
                    _undoBatches.RemoveLast();
                }
            }

            return true;
        }

        /// <summary>
        /// Registers the object and automatically watches the object. As soon as the <see cref="INotifyPropertyChanged.PropertyChanged"/> event
        /// occurs, it will automatically create a backup of the property to support undo.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        public void RegisterObject(INotifyPropertyChanged instance, object tag = null)
        {
            Argument.IsNotNull("instance", instance);

            Log.Debug("Registering object of type '{0}' with tag '{1}'", instance.GetType().Name, TagHelper.ToString(tag));

            if (!_observers.ContainsKey(instance))
            {
                _observers[instance] = new ObjectObserver(instance, tag, this);

                Log.Debug("Registered object");
            }
            else
            {
                Log.Debug("Object already registered, not registered");
            }
        }

        /// <summary>
        /// Unregisters the object and stops automatically watching the object. All undo/redo history will be removed.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        public void UnregisterObject(INotifyPropertyChanged instance)
        {
            Argument.IsNotNull("instance", instance);

            Log.Debug("Unregistering object of type '{0}'", instance.GetType().Name);

            ClearActionsForObject(instance);

            if (_observers.ContainsKey(instance))
            {
                _observers[instance].CancelSubscription();
                _observers.Remove(instance);

                Log.Debug("Unregistered object");
            }
            else
            {
                Log.Debug("Object was not registered, not unregistered");
            }
        }

        /// <summary>
        /// Registers the collection and automatically. As soon as the <see cref="INotifyCollectionChanged.CollectionChanged"/> event
        /// occurs, it will automatically create a backup of the collection to support undo.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void RegisterCollection(INotifyCollectionChanged collection, object tag = null)
        {
            Argument.IsNotNull("collection", collection);

            Log.Debug("Registering collection of type '{0}' with tag '{1}'", collection.GetType().Name, TagHelper.ToString(tag));

            if (!_observers.ContainsKey(collection))
            {
                _observers[collection] = new CollectionObserver(collection, tag, this);

                Log.Debug("Registered collection");
            }
            else
            {
                Log.Debug("Collection already registered, not registered");
            }
        }

        /// <summary>
        /// Unregisters the collection and stops automatically watching the collection. All undo/redo history will be removed.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void UnregisterCollection(INotifyCollectionChanged collection)
        {
            Argument.IsNotNull("collection", collection);

            Log.Debug("Unregistering collection of type '{0}'", collection.GetType().Name);

            ClearActionsForObject(collection);

            if (_observers.ContainsKey(collection))
            {
                _observers[collection].CancelSubscription();
                _observers.Remove(collection);

                Log.Debug("Unregistered collection");
            }
            else
            {
                Log.Debug("Collection was not registered, not unregistered");
            }
        }

        /// <summary>
        /// Clears the undo and redo actions for the specified object.
        /// </summary>
        /// <param name="obj">The object to remove the actions for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="obj"/> is <c>null</c>.</exception>
        private void ClearActionsForObject(object obj)
        {
            Argument.IsNotNull("obj", obj);

            Log.Debug("Clearing actions for object of type '{0}'", obj.GetType().Name);

            lock (_lock)
            {
                ClearActionsForObjectList(_undoBatches, obj);
                ClearActionsForObjectList(_redoBatches, obj);
            }

            Log.Debug("Cleared actions for object of type '{0}'", obj.GetType().Name);
        }

        /// <summary>
        /// Clears the undo and redo actions for the specified object for the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="obj">The object to remove the actions for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="obj"/> is <c>null</c>.</exception>
        private void ClearActionsForObjectList(List<IMementoBatch> list, object obj)
        {
            Argument.IsNotNull("obj", obj);

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var batch = list[i] as Batch;
                    if (batch != null)
                    {
                        batch.ClearActionsForObject(obj);
                        if (batch.IsEmptyBatch)
                        {
                            list.RemoveAt(i--);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clears all the undo/redo events. This should be used if some action makes the operations invalid (clearing a
        /// collection where you are tracking changes to indexes inside it for example).
        /// </summary>
        /// <param name="instance">The instance to clear the events for. If <c>null</c>, all events will be removed.</param>
        public void Clear(object instance = null)
        {
            if (instance != null)
            {
                ClearActionsForObject(instance);
                return;
            }

            Log.Debug("Clearing all actions");

            lock (_lock)
            {
                if (_undoBatches != null)
                {
                    _undoBatches.Clear();
                }

                if (_redoBatches != null)
                {
                    _redoBatches.Clear();
                }

                _isUndoingOperation = false;
            }

            Log.Debug("Cleared all actions");
        }
        #endregion
    }
}