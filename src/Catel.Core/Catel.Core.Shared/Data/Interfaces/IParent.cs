// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParent.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    /// <summary>
    /// Interface that should be implemented by all objects that can have a parent.
    /// </summary>
    public interface IParent
    {
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        IParent Parent { get; }
    }
}