// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Services
{
    using System;

#if NET
    using Microsoft.Win32;
#endif

    /// <summary>
    /// Base class for file services.
    /// </summary>
    public abstract class FileServiceBase : ViewModelServiceBase, IFileSupport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileServiceBase"/> class.
        /// </summary>
        protected FileServiceBase()
        {
#if NET
            AddExtension = true;
            CheckFileExists = false;
            CheckPathExists = true;
            FilterIndex = 1;
            ValidateNames = true;
#endif
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the filter to use when opening or saving the file.
        /// </summary>
        /// <value>The filter.</value>
        public string Filter { get; set; }

#if NET
        /// <summary>
        /// Gets or sets a value indicating whether a file dialog automatically adds an extension to a file name if the user omits an extension.
        /// </summary>
        /// <value><c>true</c> if extensions are added; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        public bool AddExtension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file dialog displays a warning if the user specifies a file name that does not exist.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>false</c>.</value>
        public bool CheckFileExists { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether warnings are displayed if the user types invalid paths and file names.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        public bool CheckPathExists { get; set; }

        /// <summary>
        /// Gets or sets the index of the filter currently selected in a file dialog.
        /// </summary>
        /// <value>The index of the selected filter. The default is <c>1</c>.</value>
        public int FilterIndex { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether the dialog accepts only valid Win32 file names.
        /// </summary>
        /// <value><c>true</c> if warnings will be shown when an invalid file name is provided; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        public bool ValidateNames { get; set; }

        /// <summary>
        /// Configures the file dialog.
        /// </summary>
        /// <param name="fileDialog">The file dialog.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fileDialog"/> is <c>null</c>.</exception>
        protected void ConfigureFileDialog(FileDialog fileDialog)
        {
            Argument.IsNotNull("fileDialog", fileDialog);

            string initialDirectory = string.Empty;
            if (!string.IsNullOrEmpty(InitialDirectory))
            {
                initialDirectory = IO.Path.AppendTrailingSlash(InitialDirectory);
            }

            fileDialog.Filter = Filter;
            fileDialog.FileName = FileName;

            fileDialog.AddExtension = AddExtension;
            fileDialog.CheckFileExists = CheckFileExists;
            fileDialog.CheckPathExists = CheckPathExists;
            fileDialog.FilterIndex = FilterIndex;
            fileDialog.InitialDirectory = initialDirectory;
            fileDialog.Title = Title;
            fileDialog.ValidateNames = ValidateNames;
        }
#endif
    }
}