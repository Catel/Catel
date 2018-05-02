// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStartUpInfoProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System.Collections.Generic;

    /// <summary>
    /// The IStartUpInfoProvider interface.
    /// </summary>
    public interface IStartUpInfoProvider
    {
#if NET
        /// <summary>
        /// Gets the application command line argument.
        /// </summary>
        string[] Arguments { get; }
#endif
    }
}