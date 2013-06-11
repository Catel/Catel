// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExportService.cs" company="Cherry development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Catel;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.MVVM.Views;
    using Catel.Services;

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

        #region IViewExportService Members

        /// <summary>
        /// Exports the <paramref name="viewModel"/>'s view to the print or clipboard or file.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="exportMode">
        /// The export mode.
        /// </param>
        /// <param name="dpiX">
        /// The dpi X.
        /// </param>
        /// <param name="dpiY">
        /// The dpi Y.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="viewModel"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// If <paramref name="exportMode"/> is <see cref="ExportMode.Print"/> then the <paramref name="dpiX"/> and <paramref name="dpiY"/> argument will be ignored.
        /// </remarks>
        public void Export(IViewModel viewModel, ExportMode exportMode = ExportMode.Print, double dpiX = 96, double dpiY = 96)
        {
            Argument.IsNotNull(() => viewModel);

            var viewManager = this.GetService<IViewManager>();
            Visual view = viewManager.GetViewsOfViewModel(viewModel).OfType<Visual>().FirstOrDefault();
            if (view == null)
            {
                string message = string.Format(CultureInfo.InvariantCulture, "There no an active view for this view model of type '{0}'", viewModel.GetType().FullName);

                Log.Error(message);

                throw new InvalidOperationException(message);
            }

            Rect bounds = VisualTreeHelper.GetDescendantBounds(view);
            var drawingVisual = new DrawingVisual();
            using (DrawingContext ctx = drawingVisual.RenderOpen())
            {
                ctx.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), bounds.Size));
            }

            if (exportMode == ExportMode.Print)
            {
                Print(drawingVisual);
            }
            else
            {
                var bitmap = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96), (int)(bounds.Height * dpiY / 96), dpiX, dpiY, PixelFormats.Pbgra32);
                bitmap.Render(drawingVisual);

                if (exportMode == ExportMode.Clipboard)
                {
                    Clipboard.SetImage(bitmap);
                }
                else
                {
                    this.SaveToFile(bitmap);
                }
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Prints a <see cref="DrawingVisual"/>.
        /// </summary>
        /// <param name="drawingVisual">
        /// The drawing visual.
        /// </param>
        private static void Print(DrawingVisual drawingVisual)
        {
            var printDialog = new PrintDialog();
            if ((bool)printDialog.ShowDialog())
            {
                printDialog.PrintVisual(drawingVisual, string.Empty);
            }
        }

        /// <summary>
        /// The save to file.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        private void SaveToFile(BitmapSource bitmap)
        {
            var saveFileService = this.GetService<ISaveFileService>();
            saveFileService.Filter = "PNG (*.png) |*.png";
            if (saveFileService.DetermineFile())
            {
                string fileName = saveFileService.FileName;
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    var encoder = new PngBitmapEncoder { Interlace = PngInterlaceOption.On };
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);
                }
            }
        }
        #endregion
    }
}