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

#if NET
    using Microsoft.Win32;
#elif NETFX_CORE

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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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
        #endregion
    }
}

#endif