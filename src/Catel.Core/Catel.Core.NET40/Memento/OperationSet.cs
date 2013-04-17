// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationSet.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Memento
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A single undo/redo operation "set" that is executed together.
    /// </summary>
    public class OperationSet : UndoBase
    {
        #region Fields
        private readonly List<IMementoSupport> _undoStack = new List<IMementoSupport>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationSet"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public OperationSet(object target, IEnumerable<IMementoSupport> operations = null, object tag = null)
            : base(target, tag)
        {
            if (operations != null)
            {
                _undoStack.AddRange(operations);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a new undo operation to the collection.
        /// </summary>
        /// <param name="operation">The operation to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="operation"/> is <c>null</c>.</exception>
        public void Add(IMementoSupport operation)
        {
            Argument.IsNotNull("operation", operation);

            _undoStack.Add(operation);

            CanRedo = _undoStack.All(op => op.CanRedo);
        }

        /// <summary>
        /// Adds a range of operations.
        /// </summary>
        /// <param name="operations">Operations to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="operations"/> is <c>null</c>.</exception>
        public void AddRange(IEnumerable<IMementoSupport> operations)
        {
            Argument.IsNotNull("operations", operations);

            _undoStack.AddRange(operations);

            CanRedo = _undoStack.All(op => op.CanRedo);
        }

        /// <summary>
        /// Method that will actually undo the action.
        /// </summary>
        protected override void UndoAction()
        {
            for (var index = _undoStack.Count - 1; index >= 0; index--)
            {
                var operation = _undoStack[index];
                operation.Undo();
            }

            CanRedo = _undoStack.All(op => op.CanRedo);
        }

        /// <summary>
        /// Method that will actually redo the action. There is no need to check for <see cref="IMementoSupport.CanRedo"/> because
        /// this will be done internally.
        /// </summary>
        protected override void RedoAction()
        {
            foreach (var operation in _undoStack)
            {
                operation.Redo();
            }

            CanRedo = _undoStack.All(op => op.CanRedo);
        }
        #endregion
    }
}