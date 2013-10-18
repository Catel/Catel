// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
#if NET
    using Microsoft.Win32;
#elif NETFX_CORE

#else
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Service to open files.
    /// </summary>
    public class OpenFileService : FileServiceBase, IOpenFileService
    {
        #region IFileSupport Members
        /// <summary>
        /// Gets the file names in case <see cref="IsMultiSelect"/> is <c>true</c>.
        /// </summary>
        /// <remarks></remarks>
        public string[] FileNames { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi select.
        /// </summary>
        /// <value><c>true</c> if this instance is multi select; otherwise, <c>false</c>.</value>
        public bool IsMultiSelect { get; set; }

#if NET
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns><c>true</c> if a file is selected; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <c>FileName</c> property will be filled with the filename. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        public virtual bool DetermineFile()
        {
            var fileDialog = new OpenFileDialog();
            ConfigureFileDialog(fileDialog);

            fileDialog.Multiselect = IsMultiSelect;

            bool result = fileDialog.ShowDialog() ?? false;
            if (result)
            {
                FileName = fileDialog.FileName;
                FileNames = fileDialog.FileNames;
            }
            else
            {
                FileName = null;
                FileNames = null;
            }

            return result;
        }
#else
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The <see cref="Stream"/> of the file or <c>null</c> if no file was selected by the user.</returns>
        /// <remarks>
        /// If this method returns a valid <see cref="Stream"/> object, the <c>FileName</c> property will be filled 
        /// with the safe filename. This can be used for display purposes only.
        /// </remarks>
        public virtual Stream[] DetermineFile()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = IsMultiSelect;
            fileDialog.Filter = Filter;

            bool result = fileDialog.ShowDialog() ?? false;
            if (result)
            {
                var files = fileDialog.Files.Select(file => file.Name).ToList();

                FileName = (files.Count > 0) ? files[0] : null;
                FileNames = (files.Count > 0) ? files.ToArray() : null;
            }
            else
            {
                FileName = null;
                FileNames = null;
            }

            var streams = fileDialog.Files.Select(file => file.OpenRead()).Cast<Stream>().ToList();
            return (streams.Count > 0) ? streams.ToArray() : null;
        }
#endif
        #endregion
    }
}
