// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INeedCustomInitialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    /// <summary>
    /// Interface that defines that a type needs initialization after it has been created by the
    /// <see cref="TypeFactory"/>.
    /// <para />
    /// As soon as a type is created, the <see cref="TypeFactory"/> will check whether it implements the <see cref="INeedCustomInitialization"/>
    /// interface. If so, it will call the <see cref="Initialize"/> method.
    /// <para />
    /// It is best to explicitly implement this interface to prevent misuse.
    /// </summary>
    public interface INeedCustomInitialization
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();
    }
}