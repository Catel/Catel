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
    public class SaveFileService : FileServiceBase, ISaveFileService
    {
#if NET
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns><c>true</c> if a file is selected; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="FileServiceBase.FileName"/> property will be filled with the filename. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        public virtual bool DetermineFile()
        {
            var fileDialog = new SaveFileDialog();
            ConfigureFileDialog(fileDialog);

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
        /// If this method returns a valid <see cref="Stream"/> object, the <c>FileName</c> property will be filled 
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
    }
}
