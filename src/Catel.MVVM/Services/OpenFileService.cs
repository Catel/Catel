// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Services
{
    using Logging;
    using System.IO;

#if NET || NETCORE
    using Microsoft.Win32;
#elif UWP

#else
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Service to open files.
    /// </summary>
    public partial class OpenFileService : FileServiceBase, IOpenFileService
    {
        #region IFileSupport Members
        /// <summary>
        /// Gets the file names in case <see cref="IsMultiSelect"/> is <c>true</c>.
        /// </summary>
        /// <remarks></remarks>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public string[] FileNames { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi select.
        /// </summary>
        /// <value><c>true</c> if this instance is multi select; otherwise, <c>false</c>.</value>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public bool IsMultiSelect { get; set; }
        #endregion
    }
}

#endif
