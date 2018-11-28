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
    [ObsoleteEx(Message = "Use Orc.CommandLine instead since that has better support for parsing command lines", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
    public interface IStartUpInfoProvider
    {
#if NET || NETCORE
        /// <summary>
        /// Gets the application command line argument.
        /// </summary>
        string[] Arguments { get; }
#endif
    }
}
