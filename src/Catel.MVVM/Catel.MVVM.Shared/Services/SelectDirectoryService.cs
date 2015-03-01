// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectDirectoryService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Services
{
    using System.Windows.Forms;

    /// <summary>
    /// Service to open files.
    /// </summary>
    public class SelectDirectoryService : ViewModelServiceBase, ISelectDirectoryService
    {
        #region ISelectDirectoryService Members
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the directory name.
        /// </summary>
        /// <value>The name of the directory.</value>
        public string DirectoryName { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the new folder button to be able to create new folders while browsing.
        /// </summary>
        /// <value><c>true</c> if the new folder button should be shown; otherwise, <c>false</c>.</value>
        public bool ShowNewFolderButton { get; set; }

        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>
        /// The initial directory.
        /// </value>
        public string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title which will be used for display.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the filter to use when opening or saving the file.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public string Filter { get; set; }

        /// <summary>
        /// Determines the DirectoryName of the Directory what will be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a directory is selected; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="DirectoryName"/> property will be filled with the directory name. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        public virtual bool DetermineDirectory()
        {   
            var browserDialog = new FolderBrowserDialog();
            browserDialog.Description = Title;
            browserDialog.ShowNewFolderButton = ShowNewFolderButton;

            var initialDirectory = InitialDirectory;

            if (!string.IsNullOrEmpty(initialDirectory))
            {
                browserDialog.SelectedPath = IO.Path.AppendTrailingSlash(initialDirectory);
            }
            else
            {
                browserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            }

            bool result = browserDialog.ShowDialog() == DialogResult.OK;
            if (result)
            {
                DirectoryName = browserDialog.SelectedPath;
            }
            else
            {
                DirectoryName = null;
            }

            return result;
        }
        #endregion
    }
}

#endif