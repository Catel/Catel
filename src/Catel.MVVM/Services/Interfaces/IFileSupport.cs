// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileSupport.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    /// <summary>
	/// Interface that supports file handling.
	/// </summary>
	public interface IFileSupport
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        string FileName { get; set; }

        /// <summary>
		/// Gets or sets the filter to use when opening or saving the file.
		/// </summary>
		/// <value>The filter.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        string Filter { get; set; }

#if NET || NETCORE
        /// <summary>
        /// Gets or sets a value indicating whether a file dialog automatically adds an extension to a file name if the user omits an extension.
        /// </summary>
        /// <value><c>true</c> if extensions are added; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        bool AddExtension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file dialog displays a warning if the user specifies a file name that does not exist.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>false</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        bool CheckFileExists { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether warnings are displayed if the user types invalid paths and file names.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        bool CheckPathExists { get; set; }

        /// <summary>
        /// Gets or sets the index of the filter currently selected in a file dialog.
        /// </summary>
        /// <value>The index of the selected filter. The default is <c>1</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        int FilterIndex { get; set; }

        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>The initial directory.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title which will be used for display.
        /// </summary>
        /// <value>The title.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog accepts only valid Win32 file names.
        /// </summary>
        /// <value><c>true</c> if warnings will be shown when an invalid file name is provided; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        bool ValidateNames { get; set; }
#endif
        #endregion
    }
}
