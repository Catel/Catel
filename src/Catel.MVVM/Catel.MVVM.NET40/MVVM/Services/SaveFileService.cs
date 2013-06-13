// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// <summary>
//   Service to save files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
#if NET
    using Microsoft.Win32;
#elif NETFX_CORE
    // TODO
#else
    using System.IO;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Service to save files.
    /// </summary>
    public class SaveFileService : ViewModelServiceBase, ISaveFileService
    {
        #region IFileSupport Members
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

#if NET
        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>The initial directory.</value>
        public string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title which will be used for display.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }
#endif

        /// <summary>
        /// Gets or sets the filter to use when opening or saving the file.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }

#if NET
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns><c>true</c> if a file is selected; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="FileName"/> property will be filled with the filename. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        public virtual bool DetermineFile()
        {
            string initialDirectory = string.Empty;
            if (!string.IsNullOrEmpty(InitialDirectory))
            {
                initialDirectory = IO.Path.AppendTrailingSlash(InitialDirectory);
            }

            var fileDialog = new SaveFileDialog();
            fileDialog.FileName = FileName;
            fileDialog.InitialDirectory = initialDirectory;
            fileDialog.Filter = Filter;
            fileDialog.Title = Title;

            bool result = fileDialog.ShowDialog() ?? false;
            if (result)
            {
                FileName = fileDialog.FileName;
            }

            return result;
        }
#elif NETFX_CORE
        // TODO Fix
#else
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The <see cref="Stream"/> of the file or <c>null</c> if no file was selected by the user..</returns>
        /// <remarks>
        /// If this method returns a valid <see cref="Stream"/> object, the <see cref="FileName"/> property will be filled 
        /// with the safe filename. This can be used for display purposes only.
        /// </remarks>
        public virtual Stream DetermineFile()
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.Filter = Filter;

            bool result = fileDialog.ShowDialog() ?? false;
            if (result)
            {
                FileName = fileDialog.SafeFileName;
            }

            return result ? fileDialog.OpenFile() : null;
        }
#endif
        #endregion
    }
}
