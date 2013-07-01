// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionChangeUndo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Memento
{
    using System;
    using System.Collections;

    /// <summary>
    /// This describes the specific change types for a collection.
    /// </summary>
    public enum CollectionChangeType
    {
        /// <summary>
        /// Item has been added to collection.
        /// </summary>
        Add,

        /// <summary>
        /// Item has been removed from collection.
        /// </summary>
        Remove,

        /// <summary>
        /// Item has been replaced within collection.
        /// </summary>
        Replace,

        /// <summary>
        /// Item has been moved inside collection.
        /// </summary>
        Move
    }

    /// <summary>
    /// This class implements the undo/redo support for collection changes. A single instance can undo one operation 
    /// performed to a collection.
    /// <para />
    /// The collection must implement <see cref="IList"/>.
    /// </summary>
    public class CollectionChangeUndo : UndoBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangeUndo"/> class.
        /// </summary>
        /// <param name="collection">Collection to work with.</param>
        /// <param name="type">Type of change.</param>
        /// <param name="oldPosition">Position of change.</param>
        /// <param name="newPosition">New position of change.</param>
        /// <param name="oldValue">Old value at position.</param>
        /// <param name="newValue">New value at position.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection" /> is <c>null</c>.</exception>
        public CollectionChangeUndo(IList collection, CollectionChangeType type, int oldPosition, int newPosition, 
            object oldValue, object newValue, object tag = null)
            : base(collection, tag)
        {
            Argument.IsNotNull("collection", collection);

            ChangeType = type;
            Position = oldPosition;
            NewPosition = newPosition;
            OldValue = oldValue;
            NewValue = newValue;

            CanRedo = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the target collection to support undo/redo for.
        /// </summary>
        public IList Collection
        {
            get { return (IList) Target; }
        }

        /// <summary>
        /// Gets the change type that has occurred.
        /// </summary>
        public CollectionChangeType ChangeType { get; private set; }

        /// <summary>
        /// Gets the position where the change occurred (old).
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets the new position where movement/insertion occurred.
        /// </summary>
        public int NewPosition { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public object NewValue { get; private set; }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public object OldValue { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Method that will actually undo the action.
        /// </summary>
        protected override void UndoAction()
        {
            switch (ChangeType)
            {
                case CollectionChangeType.Add:
                    Collection.Remove(NewValue);
                    break;

                case CollectionChangeType.Remove:
                    if (Position >= Collection.Count)
                    {
                        Collection.Add(OldValue);
                    }
                    else
                    {
                        Collection.Insert(Position, OldValue);
                    }
                    break;

                case CollectionChangeType.Replace:
                    Collection[Position] = OldValue;
                    break;

                case CollectionChangeType.Move:
                    Collection.RemoveAt(NewPosition);
                    Collection.Insert(Position, NewValue);
                    break;
            }
        }

        /// <summary>
        /// Method that will actually redo the action.
        /// </summary>
        protected override void RedoAction()
        {
            switch (ChangeType)
            {
                case CollectionChangeType.Remove:
                    Collection.Remove(OldValue);
                    break;

                case CollectionChangeType.Add:
                    if (NewPosition == -1 || NewPosition >= Collection.Count)
                    {
                        Collection.Add(NewValue);
                    }
                    else
                    {
                        Collection.Insert(NewPosition, NewValue);
                    }
                    break;

                case CollectionChangeType.Replace:
                    Collection[Position] = NewValue;
                    break;

                case CollectionChangeType.Move:
                    Collection.RemoveAt(Position);
                    if (NewPosition == -1 || NewPosition >= Collection.Count)
                    {
                        Collection.Add(NewValue);
                    }
                    else
                    {
                        Collection.Insert(NewPosition, NewValue);
                    }
                    break;
            }
        }
        #endregion
    }
}