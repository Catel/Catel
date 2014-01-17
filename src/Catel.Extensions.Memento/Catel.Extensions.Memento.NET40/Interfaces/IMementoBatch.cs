// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMementoBatch.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Memento
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a batch of memento actions.
    /// </summary>
    public interface IMementoBatch
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; set; }

        /// <summary>
        /// Gets the action count.
        /// </summary>
        /// <value>The action count.</value>
        int ActionCount { get; }

        /// <summary>
        /// Gets a value indicating whether this is an empty batch, meaning it contains no actions.
        /// </summary>
        /// <value><c>true</c> if this batch is empty; otherwise, <c>false</c>.</value>
        bool IsEmptyBatch { get; }

        /// <summary>
        /// Gets a value indicating whether this is a single action batch, meaning it only contains one action.
        /// </summary>
        /// <value><c>true</c> if this is a single action batch; otherwise, <c>false</c>.</value>
        bool IsSingleActionBatch { get; }

        /// <summary>
        /// Gets the actions that belong to this batch.
        /// </summary>
        /// <value>The actions.</value>
        IEnumerable<IMementoSupport> Actions { get; }

        /// <summary>
        /// Gets a value indicating whether at least one action in this batch can redo.
        /// </summary>
        /// <value><c>true</c> if at least one action in this batch can redo; otherwise, <c>false</c>.</value>
        bool CanRedo { get; }

        /// <summary>
        /// Calls the <see cref="IMementoSupport.Undo"/> of all actions in this batch.
        /// </summary>
        void Undo();

        /// <summary>
        /// Calls the <see cref="IMementoSupport.Redo"/> of all actions in this batch.
        /// </summary>
        void Redo();
    }
}