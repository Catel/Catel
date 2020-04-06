// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISelectDirectoryService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Services
{
    using System.Threading.Tasks;

#if UWP
    using global::Windows.Storage;
#endif

    /// <summary>
    /// Interface for the Select Directory service.
    /// </summary>
    public interface ISelectDirectoryService
    {
        #region Methods
#if UWP
        /// <summary>
        /// Determines the name of the directory what will be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a directory is selected; otherwise <c>false</c>.
        /// </returns>
        Task<DetermineDirectoryResult> DetermineDirectoryAsync(DetermineDirectoryContext context);
#else
        /// <summary>
        /// Determines the name of the directory what will be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a directory is selected; otherwise <c>false</c>.
        /// </returns>
        Task<DetermineDirectoryResult> DetermineDirectoryAsync(DetermineDirectoryContext context);
#endif
        #endregion
    }
}
