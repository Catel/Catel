// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewExportService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System.Threading.Tasks;
    using Catel.MVVM;
    
    /// <summary>
    /// The export mode.
    /// </summary>
    public enum ExportMode
    {
        /// <summary>
        /// The print export mode.
        /// </summary>
        Print,

#if NET || NETCORE
        /// <summary>
        /// The clipboard export mode.
        /// </summary>
        Clipboard,
#endif

        /// <summary>
        /// The file export mode
        /// </summary>
        File
    }

    /// <summary>
    /// The ViewExportService interface.
    /// </summary>
    public interface IViewExportService
    {
        #region Methods
        /// <summary>
        /// Exports the <paramref name="viewModel" />'s view to the print or clipboard or file.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="exportMode">The export mode.</param>
        /// <param name="dpiX">The dpi X.</param>
        /// <param name="dpiY">The dpi Y.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <remarks>
        /// If <paramref name="exportMode" /> is <see cref="ExportMode.Print" /> then the <paramref name="dpiX" /> and <paramref name="dpiY" /> argument will be ignored.
        /// </remarks>
        Task ExportAsync(IViewModel viewModel, ExportMode exportMode = ExportMode.Print, double dpiX = 96, double dpiY = 96);
        #endregion
    }
}
