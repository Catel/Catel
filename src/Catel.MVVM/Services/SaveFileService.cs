// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Services
{
    using Logging;

    /// <summary>
    /// Service to save files.
    /// </summary>
    public partial class SaveFileService : FileServiceBase, ISaveFileService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
    }
}

#endif