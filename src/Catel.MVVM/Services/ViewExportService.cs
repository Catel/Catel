// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExportService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Services
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media.Imaging;

    using Catel.Logging;
    using Catel.MVVM;
    using Catel.MVVM.Views;
    using Catel.Services;

    using System.Windows.Media;
    using System.Threading.Tasks;

    /// <summary>
    /// The ViewExportService interface.
    /// </summary>
    public class ViewExportService : ServiceBase, IViewExportService
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly IViewManager _viewManager;
        private readonly ISaveFileService _saveFileService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewExportService" /> class.
        /// </summary>
        /// <param name="viewManager">The view manager.</param>
        /// <param name="saveFileService">The save file service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewManager" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="saveFileService" /> is <c>null</c>.</exception>
        public ViewExportService(IViewManager viewManager, ISaveFileService saveFileService)
        {
            Argument.IsNotNull("viewManager", viewManager);
            Argument.IsNotNull("saveFileService", saveFileService);

            _viewManager = viewManager;
            _saveFileService = saveFileService;
        }

        #region IViewExportService Members
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
        public virtual async Task ExportAsync(IViewModel viewModel, ExportMode exportMode = ExportMode.Print, double dpiX = 96, double dpiY = 96)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var view = _viewManager.GetViewsOfViewModel(viewModel).OfType<UIElement>().FirstOrDefault();
            if (view == null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("There no an active view for this view model of type '{0}'", viewModel.GetType().FullName);
            }

            var bitmap = CreateImageFromUIElement(view, dpiX, dpiY);

            if (exportMode == ExportMode.Print)
            {
                Print(bitmap);
            }
            else
            {
                if (exportMode == ExportMode.File)
                {
                    await SaveToFileAsync(bitmap);
                }
                else
                {
                    Clipboard.SetImage(bitmap);
                }
            }
        }

        /// <summary>
        /// Exports the <paramref name="viewModel" />'s view to the print or clipboard or file.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="exportMode">The export mode.</param>
        /// <param name="dpiX">The dpi X.</param>
        /// <param name="dpiY">The dpi Y.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <remarks>If <paramref name="exportMode" /> is <see cref="ExportMode.Print" /> then the <paramref name="dpiX" /> and <paramref name="dpiY" /> argument will be ignored.</remarks>
        [ObsoleteEx(ReplacementTypeOrMember = nameof(ExportAsync), TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public virtual async void Export(IViewModel viewModel, ExportMode exportMode = ExportMode.Print, double dpiX = 96, double dpiY = 96)
        {
            await ExportAsync(viewModel, exportMode, dpiX, dpiY);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prints a <see cref="UIElement" />.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        private static void Print(BitmapSource bitmap)
        {
            var image = new Image();
            image.Source = bitmap;

            var printDialog = new PrintDialog();
            if ((bool)printDialog.ShowDialog())
            {
                printDialog.PrintVisual(image, string.Empty);
            }
        }

        /// <summary>
        /// The save to file.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        private async Task SaveToFileAsync(BitmapSource bitmap)
        {
            _saveFileService.Filter = "PNG (*.png) |*.png";
            if (await _saveFileService.DetermineFileAsync())
            {
                var fileName = _saveFileService.FileName;
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    var encoder = new PngBitmapEncoder { Interlace = PngInterlaceOption.On };
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);
                }
            }
        }

        private static BitmapSource CreateImageFromUIElement(UIElement element, double dpiX, double dpiY)
        {
            var bitmap = new RenderTargetBitmap((int)(element.RenderSize.Width * dpiX / 96),
                (int)(element.RenderSize.Height * dpiY / 96), dpiX, dpiY, PixelFormats.Pbgra32);
            bitmap.Render(element);

            return bitmap;
        }
        #endregion
    }
}

#endif
