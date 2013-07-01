// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UndoBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Memento
{
    using System;
    using Logging;

    /// <summary>
    /// Base class for all classes implementing the <see cref="IMementoSupport" /> interface.
    /// </summary>
    public abstract class UndoBase : IMementoSupport
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoBase"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        protected UndoBase(object target, object tag)
        {
            Argument.IsNotNull("target", target);

            Target = target;
            Tag = tag;

            Log.Debug("Constructed '{0}' undo/redo action for object of type '{0}' with tag '{1}'", GetType().Name, target.GetType().Name, TagHelper.ToString(Tag));
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public object Target { get; private set; }

        /// <summary>
        /// Gets or sets the description, which is option and can be used to display a text to the end-user.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tag which can be used to group operations by object.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation can be "reapplied" after undo.
        /// </summary>
        /// <value><c>true</c> if this instance can redo; otherwise, <c>false</c>.</value>
        public bool CanRedo { get; protected set; }

        /// <summary>
        /// Method used to undo operation.
        /// </summary>
        public void Undo()
        {
            Log.Debug("Undoing action '{0}' for object with tag '{1}'", GetType().Name, TagHelper.ToString(Tag));

            UndoAction();

            Log.Debug("Undoed action '{0}' for object with tag '{1}'", GetType().Name, TagHelper.ToString(Tag));    
        }

        /// <summary>
        /// Method that will actually undo the action.
        /// </summary>
        protected abstract void UndoAction();

        /// <summary>
        /// Method to redo operation.
        /// </summary>
        public void Redo()
        {
            if (!CanRedo)
            {
                Log.Info("Cannot redo action '{0}' for object with tag '{1}'", GetType().Name, TagHelper.ToString(Tag));
                return;
            }

            Log.Debug("Redoing action '{0}' for object with tag '{1}'", GetType().Name, TagHelper.ToString(Tag));

            RedoAction();

            Log.Debug("Redoed action '{0}' for object with tag '{1}'", GetType().Name, TagHelper.ToString(Tag));   
        }

        /// <summary>
        /// Method that will actually redo the action. There is no need to check for <see cref="IMementoSupport.CanRedo"/> because
        /// this will be done internally.
        /// </summary>
        protected abstract void RedoAction();
    }
}