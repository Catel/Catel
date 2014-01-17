// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStartUpInfoProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System.Collections.Generic;

    /// <summary>
    /// The IStartUpInfoProvider interface.
    /// </summary>
    public interface IStartUpInfoProvider
    {
#if SILVERLIGHT
        /// <summary>
        /// Gets the silverlight application initialization parameters.
        /// </summary>
        IDictionary<string, string> InitParms { get; }
#endif

#if NET
        /// <summary>
        /// Gets the application command line argument.
        /// </summary>
        string[] Arguments { get; }
#endif
    }
}