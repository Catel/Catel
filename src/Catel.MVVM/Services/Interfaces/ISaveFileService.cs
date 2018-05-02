// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISaveFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System.Threading.Tasks;
    
#if NETFX_CORE
    using System.IO;
    using global::Windows.Storage;
#endif

    /// <summary>
	/// Interface for the Save File service.
	/// </summary>
	public interface ISaveFileService : IFileSupport
	{
#if NETFX_CORE
		/// <summary>
		/// Determines the filename of the file what will be used.
		/// </summary>
		/// <returns>The <see cref="StorageFile"/> of the file or <c>null</c> if no file was selected by the user.</returns>
		/// <remarks>
		/// If this method returns a valid <see cref="StorageFile"/> object, the <see cref="IFileSupport.FileName"/> property will be filled 
		/// with the safe filename. This can be used for display purposes only.
		/// </remarks>
		Task<StorageFile> DetermineFileAsync();
#else
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns><c>true</c> if a file is selected; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="IFileSupport.FileName"/> property will be filled with the filenames. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        Task<bool> DetermineFileAsync();
#endif
	}
}
