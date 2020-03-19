// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISelectDirectoryService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Services
{
    using System.Threading.Tasks;

#if UWP
    using global::Windows.Storage;
#endif

    /// <summary>
    /// Interface for the Select Directory service.
    /// </summary>
    public interface ISelectDirectoryService
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryContext / DetermineDirectoryResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        string FileName { get; set; }

        /// <summary>
        /// Gets or sets the filter to use when opening or saving the file.
        /// </summary>
        /// <value>The filter.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryContext / DetermineDirectoryResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        string Filter { get; set; }

        /// <summary>
        /// Gets the directory name.
        /// </summary>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryContext / DetermineDirectoryResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        string DirectoryName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the new folder button to be able to create new folders while browsing.
        /// </summary>
        /// <value><c>true</c> if the new folder button should be shown; otherwise, <c>false</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryContext / DetermineDirectoryResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        bool ShowNewFolderButton { get; set; }

        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>The initial directory.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryContext / DetermineDirectoryResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title which will be used for display.
        /// </summary>
        /// <value>The title.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryContext / DetermineDirectoryResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        string Title { get; set; }
        #endregion

        #region Methods
#if UWP
        /// <summary>
        /// Determines the name of the directory what will be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a directory is selected; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="DirectoryName"/> property will be filled with the directory name. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryAsync(DetermineDirectoryContext)", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        Task<StorageFolder> DetermineDirectoryAsync();
#else
        /// <summary>
        /// Determines the name of the directory what will be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a directory is selected; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="DirectoryName"/> property will be filled with the directory name. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryAsync(DetermineDirectoryContext)", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        Task<bool> DetermineDirectoryAsync();
#endif

#if UWP
        /// <summary>
        /// Determines the name of the directory what will be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a directory is selected; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="DirectoryName"/> property will be filled with the directory name. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        Task<DetermineDirectoryResult> DetermineDirectoryAsync(DetermineDirectoryContext context);
#else
        /// <summary>
        /// Determines the name of the directory what will be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a directory is selected; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="DirectoryName"/> property will be filled with the directory name. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        Task<DetermineDirectoryResult> DetermineDirectoryAsync(DetermineDirectoryContext context);
#endif
        #endregion
    }
}
