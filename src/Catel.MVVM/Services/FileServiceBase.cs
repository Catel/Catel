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

        }

        /// <summary>
        /// Gets the initial directory used for the file dialog.
        /// </summary>
        /// <returns>The inital directory.</returns>
        protected virtual string GetInitialDirectory(DetermineFileContext context)
        {
            Argument.IsNotNull(() => context);

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

#if NET || NETCORE
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
