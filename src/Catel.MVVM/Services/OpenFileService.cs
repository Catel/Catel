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

    }
}

#endif
