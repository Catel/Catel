// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Batch.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Memento
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

    /// <summary>
    /// Represents a batch of memento actions.
    /// </summary>
    public class Batch : IMementoBatch, IUniqueIdentifyable
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly List<IMementoSupport> _actions = new List<IMementoSupport>();

        private bool _canRedo;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Batch"/> class.
        /// </summary>
        public Batch()
        {
            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<Batch>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int UniqueIdentifier { get; private set; }
        #endregion

        #region IMementoBatch Members
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the action count.
        /// </summary>
        /// <value>
        /// The action count.
        /// </value>
        public int ActionCount
        {
            get { return _actions.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this is an empty batch, meaning it contains no actions.
        /// </summary>
        /// <value>
        /// <c>true</c> if this batch is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmptyBatch
        {
            get { return _actions.Count == 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this is a single action batch, meaning it only contains one action.
        /// </summary>
        /// <value>
        /// <c>true</c> if this is a single action batch; otherwise, <c>false</c>.
        /// </value>
        public bool IsSingleActionBatch
        {
            get { return _actions.Count == 1; }
        }

        /// <summary>
        /// Gets the actions that belong to this batch.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        public IEnumerable<IMementoSupport> Actions { get; private set; }

        /// <summary>
        /// Gets a value indicating whether at least one action in this batch can redo.
        /// </summary>
        /// <value>
        /// <c>true</c> if at least one action in this batch can redo; otherwise, <c>false</c>.
        /// </value>
        public bool CanRedo
        {
            get { return _canRedo; }
        }

        /// <summary>
        /// Calls the <see cref="IMementoSupport.Undo"/> of all actions in this batch.
        /// </summary>
        public void Undo()
        {
            foreach (var action in _actions)
            {
                action.Undo();
            }
        }

        /// <summary>
        /// Calls the <see cref="IMementoSupport.Redo"/> of all actions in this batch.
        /// </summary>
        public void Redo()
        {
            foreach (var action in _actions)
            {
                action.Redo();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the action to the current batch.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public void AddAction(IMementoSupport action)
        {
            Argument.IsNotNull("action", action);

            _actions.Add(action);

            if (IsSingleActionBatch)
            {
                _canRedo = action.CanRedo;
            }
            else
            {
                _canRedo = (_canRedo && action.CanRedo);
            }
        }

        /// <summary>
        /// Clears the undo and redo actions for the specified object.
        /// </summary>
        /// <param name="obj">The object to remove the actions for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        public void ClearActionsForObject(object obj)
        {
            Argument.IsNotNull("obj", obj);

            Log.Debug("Clearing actions for object of type '{0}' in batch '{1}'", obj.GetType().Name, UniqueIdentifier);

            if (_actions != null)
            {
                var temp = new List<IMementoSupport>(_actions.Where(operation => operation.Target == obj));
                foreach (var operation in temp)
                {
                    _actions.Remove(operation);
                }
            }

            Log.Debug("Cleared actions for object of type '{0}' in batch '{1}'", obj.GetType().Name, UniqueIdentifier);
        }
        #endregion
    }
}