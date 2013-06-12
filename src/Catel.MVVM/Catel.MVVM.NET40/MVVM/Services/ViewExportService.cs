// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExportService.cs" company="Catel development team">
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
    using System.Windows.Media.Imaging;

    using Catel.Logging;
    using Catel.MVVM.Views;
    using Catel.Services;

#if SILVERLIGHT
    using System.Windows.Printing;
#else
    using System.Windows.Controls;
    using System.Windows.Media;
#endif

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
        /// Exports the <paramref name="viewModel" />'s view to the print or clipboard or file.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="exportMode">The export mode.</param>
        /// <param name="dpiX">The dpi X.</param>
        /// <param name="dpiY">The dpi Y.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <remarks>If <paramref name="exportMode" /> is <see cref="ExportMode.Print" /> then the <paramref name="dpiX" /> and <paramref name="dpiY" /> argument will be ignored.</remarks>
        public virtual void Export(IViewModel viewModel, ExportMode exportMode = ExportMode.Print, double dpiX = 96, double dpiY = 96)
        {
            Argument.IsNotNull(() => viewModel);

            var viewManager = GetService<IViewManager>();
            var view = viewManager.GetViewsOfViewModel(viewModel).OfType<UIElement>().FirstOrDefault();
            if (view == null)
            {
                string message = string.Format(CultureInfo.InvariantCulture, "There no an active view for this view model of type '{0}'", viewModel.GetType().FullName);

                Log.Error(message);

                throw new InvalidOperationException(message);
            }

            if (exportMode == ExportMode.Print)
            {
                Print(view);
            }
            else
            {
                var bitmap = CreateImageFromUIElement(view, dpiX, dpiY);

                switch (exportMode)
                {
                    case ExportMode.File:
                        SaveToFile(bitmap);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("exportMode");
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prints a <see cref="UIElement" />.
        /// </summary>
        /// <param name="visual">The visual.</param>
        private static void Print(UIElement visual)
        {
#if SILVERLIGHT
            var printDocument = new PrintDocument();
            printDocument.PrintPage += (s, e) => { e.PageVisual = visual; };
            printDocument.Print("Silverlight printed document");
#else
            var printDialog = new PrintDialog();
            if ((bool)printDialog.ShowDialog())
            {
                printDialog.PrintVisual(visual, string.Empty);
            }
#endif
        }

        /// <summary>
        /// The save to file.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        private void SaveToFile(BitmapSource bitmap)
        {
            var saveFileService = GetService<ISaveFileService>();

#if SILVERLIGHT
            saveFileService.Filter = "BMP (*.bmp) |*.bmp";
            using (var stream = saveFileService.DetermineFile())
            {
                if (stream != null)
                {
                    using (var streamWriter = new StreamWriter(stream))
                    {
                        var writeableBitmap = new WriteableBitmap(bitmap);
                        var byteArray = ConvertWritableBitmapToByteArray(writeableBitmap);
                        streamWriter.Write(byteArray);
                    }
                }
            }
#else
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
#endif
        }

        private static BitmapSource CreateImageFromUIElement(UIElement element, double dpiX, double dpiY)
        {
#if SILVERLIGHT
            var bitmap = new WriteableBitmap((int)element.RenderSize.Width, (int)element.RenderSize.Height);

            Array.Clear(bitmap.Pixels, 0, bitmap.Pixels.Length);
            bitmap.Render(element, element.RenderTransform);
            bitmap.Invalidate();            
#else
            var bitmap = new RenderTargetBitmap((int)(element.RenderSize.Width * dpiX / 96),
                (int)(element.RenderSize.Height * dpiY / 96), dpiX, dpiY, PixelFormats.Pbgra32);
            bitmap.Render(element);
#endif

            return bitmap;
        }

#if SILVERLIGHT
        private static byte[] ConvertWritableBitmapToByteArray(WriteableBitmap bmp)
        {
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] p = bmp.Pixels;
            int len = p.Length;
            byte[] result = new byte[4 * w * h];

            // Copy pixels to buffer
            for (int i = 0, j = 0; i < len; i++, j += 4)
            {
                int color = p[i];
                result[j + 0] = (byte)(color >> 24); // A
                result[j + 1] = (byte)(color >> 16); // R
                result[j + 2] = (byte)(color >> 8);  // G
                result[j + 3] = (byte)(color);       // B
            }

            return result;
        }
#endif
        #endregion
    }
}