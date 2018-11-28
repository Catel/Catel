// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectDirectoryService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Services
{
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Service to open files.
    /// </summary>
    public partial class SelectDirectoryService
    {
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
        public virtual async Task<bool> DetermineDirectoryAsync()
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
    }
}

#endif
