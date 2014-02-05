// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMementoService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Memento
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// This interface describes a simple Undo service.
    /// </summary>
    public interface IMementoService
    {
        #region Properties
        /// <summary>
        /// Gets or sets the maximum supported batches.
        /// </summary>
        /// <value>The maximum supported batches.</value>
        int MaximumSupportedBatches { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is at least one Undo operation we can perform.
        /// </summary>
        /// <value><c>true</c> if this instance can undo; otherwise, <c>false</c>.</value>
        bool CanUndo { get; }

        /// <summary>
        /// Gets a value indicating whether there is at least one Redo operation we can perform.
        /// </summary>
        /// <value><c>true</c> if this instance can redo; otherwise, <c>false</c>.</value>
        bool CanRedo { get; }

        /// <summary>
        /// Gets the redo batches.
        /// </summary>
        /// <value>The redo batches.</value>
        IEnumerable<IMementoBatch> RedoBatches { get; }

        /// <summary>
        /// Gets the undo batches.
        /// </summary>
        /// <value>The undo batches.</value>
        IEnumerable<IMementoBatch> UndoBatches { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the service is enabled.
        /// </summary>
        /// <value><c>true</c> if the service is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Begins a new batch.
        /// <para />
        /// Note that this method will always call <see cref="EndBatch" /> before creating the new batch to ensure
        /// that a new batch is actually created.
        /// <para />
        /// All operations added via the <see cref="Add(Catel.Memento.IMementoSupport,bool)" /> will belong the this batch
        /// and be handled as a single operation.
        /// </summary>
        /// <param name="title">The title which can be used to display this batch i a user interface.</param>
        /// <param name="description">The description which can be used to display this batch i a user interface.</param>
        /// <returns>The <see cref="IMementoBatch" /> that has just been created.</returns>
        IMementoBatch BeginBatch(string title = null, string description = null);

        /// <summary>
        /// Ends the current batch and adds it to the stack by calling <see cref="Add(Catel.Memento.IMementoBatch,bool)"/>.
        /// <para />
        /// If there is currently no batch, this method will silently exit.
        /// </summary>
        /// <returns>The <see cref="IMementoBatch"/> that has just been ended or <c>null</c> if there was no current batch.</returns>
        IMementoBatch EndBatch();

        /// <summary>
        /// Executes the next Undo batch.
        /// </summary>
        /// <returns><c>true</c> if an undo was executed.</returns>
        bool Undo();

        /// <summary>
        /// Executes the last Redo batch.
        /// </summary>
        /// <returns><c>true</c> if a redo occurred.</returns>
        bool Redo();

        /// <summary>
        /// Adds a new undo operation to the stack.
        /// <para />
        /// Note that this method does not respect the current batch. When a current batch is in the making, this
        /// batch will be inserted in front of the batch. A batch will be added to the stack as soon as it is ended.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="noInsertIfExecutingOperation">Do not insert record if currently running undo/redo.</param>
        /// <returns><c>true</c> if record inserted; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="operation"/> is <c>null</c>.</exception>
        bool Add(IMementoSupport operation, bool noInsertIfExecutingOperation = true);

        /// <summary>
        /// Adds a new batch to the stack.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="noInsertIfExecutingOperation">Do not insert record if currently running undo/redo.</param>
        /// <returns><c>true</c> if record inserted; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="batch"/> is <c>null</c>.</exception>
        bool Add(IMementoBatch batch, bool noInsertIfExecutingOperation = true);

        /// <summary>
        /// Registers the object and automatically watches the object. As soon as the <see cref="INotifyPropertyChanged.PropertyChanged" /> event
        /// occurs, it will automatically create a backup of the property to support undo.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <remarks>The <see cref="RegisterObject" /> will subscribe to the instance using the <see cref="IWeakEventListener" />. There is no
        /// need to use the <see cref="UnregisterObject" /> unless an object must be cleared manually.</remarks>
        void RegisterObject(INotifyPropertyChanged instance, object tag = null);

        /// <summary>
        /// Unregisters the object and stops automatically watching the object. All undo/redo history will be removed.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <remarks>The <see cref="RegisterObject" /> will subscribe to the instance using the <see cref="IWeakEventListener" />. There is no
        /// need to use the <see cref="UnregisterObject" /> unless an object must be cleared manually.</remarks>
        void UnregisterObject(INotifyPropertyChanged instance);

        /// <summary>
        /// Registers the collection and automatically. As soon as the <see cref="INotifyCollectionChanged.CollectionChanged" /> event
        /// occurs, it will automatically create a backup of the collection to support undo.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> is <c>null</c>.</exception>
        void RegisterCollection(INotifyCollectionChanged collection, object tag = null);

        /// <summary>
        /// Unregisters the collection and stops automatically watching the collection. All undo/redo history will be removed.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> is <c>null</c>.</exception>
        void UnregisterCollection(INotifyCollectionChanged collection);

        /// <summary>
        /// Clears all the undo/redo events. This should be used if some action makes the operations invalid (clearing a
        /// collection where you are tracking changes to indexes inside it for example).
        /// </summary>
        /// <param name="instance">The instance to clear the events for. If <c>null</c>, all events will be removed.</param>
        void Clear(object instance = null);
        #endregion
    }
}