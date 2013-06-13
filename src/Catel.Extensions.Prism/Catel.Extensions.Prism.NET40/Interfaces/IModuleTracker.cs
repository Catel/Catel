// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModuleTracker.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    /// <summary>
    /// Provides ability for modules to inform the quickstart of their state.
    /// </summary>
    public interface IModuleTracker
    {
        /// <summary>
        /// Records the module is loading.
        /// </summary>
        /// <param name="moduleName">The well-known name of the module.</param>
        /// <param name="bytesReceived">The number of bytes downloaded.</param>
        /// <param name="totalBytesToReceive">The total bytes to receive.</param>
        void RecordModuleDownloading(string moduleName, long bytesReceived, long totalBytesToReceive);

        /// <summary>
        /// Records the module has been loaded.
        /// </summary>
        /// <param name="moduleName">The well-known name of the module.</param>
        void RecordModuleLoaded(string moduleName);

        /// <summary>
        /// Records that the module has been constructed.
        /// </summary>
        /// <param name="moduleName">The well-known name of the module.</param>
        void RecordModuleConstructed(string moduleName);

        /// <summary>
        /// Records that the module has been initialized.
        /// </summary>
        /// <param name="moduleName">The well-known name of the module.</param>
        void RecordModuleInitialized(string moduleName);
    }
}