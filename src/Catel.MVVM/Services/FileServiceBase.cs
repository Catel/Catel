// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.IO;

#if NET || NETCORE
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
#if NET || NETCORE
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
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the filter to use when opening or saving the file.
        /// </summary>
        /// <value>The filter.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public string Filter { get; set; }

#if NET || NETCORE
        /// <summary>
        /// Gets or sets a value indicating whether a file dialog automatically adds an extension to a file name if the user omits an extension.
        /// </summary>
        /// <value><c>true</c> if extensions are added; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public bool AddExtension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file dialog displays a warning if the user specifies a file name that does not exist.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>false</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public bool CheckFileExists { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether warnings are displayed if the user types invalid paths and file names.
        /// </summary>
        /// <value><c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public bool CheckPathExists { get; set; }

        /// <summary>
        /// Gets or sets the index of the filter currently selected in a file dialog.
        /// </summary>
        /// <value>The index of the selected filter. The default is <c>1</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public int FilterIndex { get; set; }

        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>The initial directory.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title which will be used for display.
        /// </summary>
        /// <value>The title.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog accepts only valid Win32 file names.
        /// </summary>
        /// <value><c>true</c> if warnings will be shown when an invalid file name is provided; otherwise, <c>false</c>. The default is <c>true</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public bool ValidateNames { get; set; }

        /// <summary>
        /// Gets the initial directory used for the file dialog.
        /// <para />
        /// The default implementation not only uses the <see cref="InitialDirectory"/> property, but also
        /// checks whether it actually exists on disk to prevent exceptions.
        /// </summary>
        /// <returns>The inital directory.</returns>
        [ObsoleteEx(ReplacementTypeOrMember = "GetInitialDirectory(DetermineFileContext)", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        protected virtual string GetInitialDirectory()
        {
            var initialDirectory = InitialDirectory;
            if (!string.IsNullOrWhiteSpace(initialDirectory))
            {
                if (!Directory.Exists(initialDirectory))
                {
                    initialDirectory = null;
                }
                else
                {
                    initialDirectory = IO.Path.AppendTrailingSlash(initialDirectory);
                }
            }

            return initialDirectory;
        }

        /// <summary>
        /// Gets the initial directory used for the file dialog.
        /// <para />
        /// The default implementation not only uses the <see cref="InitialDirectory"/> property, but also
        /// checks whether it actually exists on disk to prevent exceptions.
        /// </summary>
        /// <returns>The inital directory.</returns>
        protected virtual string GetInitialDirectory(DetermineFileContext context)
        {
            Argument.IsNotNull(nameof(context), context);

            var initialDirectory = context.InitialDirectory;
            if (!string.IsNullOrWhiteSpace(initialDirectory))
            {
                if (!Directory.Exists(initialDirectory))
                {
                    initialDirectory = null;
                }
                else
                {
                    initialDirectory = IO.Path.AppendTrailingSlash(initialDirectory);
                }
            }

            return initialDirectory;
        }

        /// <summary>
        /// Configures the file dialog.
        /// </summary>
        /// <param name="fileDialog">The file dialog.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fileDialog"/> is <c>null</c>.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "ConfigureFileDialog(FileDialog, DetermineFileContext)", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        protected virtual void ConfigureFileDialog(FileDialog fileDialog)
        {
            Argument.IsNotNull("fileDialog", fileDialog);

            fileDialog.Filter = Filter;
            fileDialog.FileName = FileName;

            fileDialog.AddExtension = AddExtension;
            fileDialog.CheckFileExists = CheckFileExists;
            fileDialog.CheckPathExists = CheckPathExists;
            fileDialog.FilterIndex = FilterIndex;
            fileDialog.InitialDirectory = GetInitialDirectory();
            fileDialog.Title = Title;
            fileDialog.ValidateNames = ValidateNames;
        }

        /// <summary>
        /// Configures the file dialog.
        /// </summary>
        /// <param name="fileDialog">The file dialog.</param>
        /// <param name="context">The determine file context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fileDialog"/> is <c>null</c>.</exception>
        protected virtual void ConfigureFileDialog(FileDialog fileDialog, DetermineFileContext context)
        {
            Argument.IsNotNull("fileDialog", fileDialog);
            Argument.IsNotNull("context", context);

            fileDialog.Filter = context.Filter;
            fileDialog.FileName = context.FileName;

            fileDialog.AddExtension = context.AddExtension;
            fileDialog.CheckFileExists = context.CheckFileExists;
            fileDialog.CheckPathExists = context.CheckPathExists;
            fileDialog.FilterIndex = context.FilterIndex;
            fileDialog.InitialDirectory = GetInitialDirectory(context);
            fileDialog.Title = context.Title;
            fileDialog.ValidateNames = context.ValidateNames;
        }
#endif
    }
}
