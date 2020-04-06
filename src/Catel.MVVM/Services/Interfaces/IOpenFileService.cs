// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOpenFileService.cs" company="Catel development team">
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
    /// Interface for the Open File service.
    /// </summary>
    public interface IOpenFileService : IFileSupport
	{
#if UWP
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The determine open file result.</returns>
        Task<DetermineOpenFileResult> DetermineFileAsync(DetermineOpenFileContext context);
#else
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The determine open file result.</returns>
        Task<DetermineOpenFileResult> DetermineFileAsync(DetermineOpenFileContext context);
#endif
	}
}
