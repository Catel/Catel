// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISaveFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System.Threading.Tasks;

#if UWP
    using System.IO;
    using global::Windows.Storage;
#endif

    /// <summary>
    /// Interface for the Save File service.
    /// </summary>
    public interface ISaveFileService : IFileSupport
	{
#if UWP
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The determine save file result.</returns>
        Task<DetermineSaveFileResult> DetermineFileAsync(DetermineSaveFileContext context);
#else
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The determine save file result.</returns>
        Task<DetermineSaveFileResult> DetermineFileAsync(DetermineSaveFileContext context);
#endif
    }
}
