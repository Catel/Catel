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
        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi select.
        /// </summary>
        /// <value><c>true</c> if this instance is multi select; otherwise, <c>false</c>.</value>
        bool IsMultiSelect { get; set; }

        /// <summary>
        /// Gets the file names in case <see cref="IsMultiSelect"/> is <c>true</c>.
        /// </summary>
        string[] FileNames { get; }

#if UWP
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The <see cref="StorageFile"/> of the file or <c>null</c> if no file was selected by the user.</returns>
        /// <remarks>
        /// If this method returns a valid <see cref="Stream"/> object, the <see cref="FileNames"/> property will be filled 
        /// with the safe filename. This can be used for display purposes only.
        /// </remarks>
        Task<StorageFile[]> DetermineFileAsync();
#else
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns><c>true</c> if a file is selected; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="FileNames"/> property will be filled with the filenames. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        Task<bool> DetermineFileAsync();
#endif
	}
}
