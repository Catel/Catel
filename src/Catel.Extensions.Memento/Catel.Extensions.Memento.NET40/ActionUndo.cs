// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionUndo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Memento
{
    using System;

    /// <summary>
    /// This class implements a generic undo using delegates.
    /// </summary>
    public class ActionUndo : UndoBase
    {
        #region Fields
        private readonly Action _redoAction;
        private readonly Action _undoAction;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionUndo" /> class.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="undoAction">Action for undo.</param>
        /// <param name="redoAction">Optional action for redo.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="undoAction" /> is <c>null</c>.</exception>
        public ActionUndo(object target, Action undoAction, Action redoAction = null, object tag = null)
            : base(target, tag)
        {
            Argument.IsNotNull("undoAction", undoAction);

            _undoAction = undoAction;
            _redoAction = redoAction;

            CanRedo = (_redoAction != null);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Method that will actually undo the action.
        /// </summary>
        protected override void UndoAction()
        {
            _undoAction.Invoke();
        }

        /// <summary>
        /// Method that will actually redo the action. There is no need to check for <see cref="IMementoSupport.CanRedo"/> because
        /// this will be done internally.
        /// </summary>
        protected override void RedoAction()
        {
            _redoAction.Invoke();
        }
        #endregion
    }
}