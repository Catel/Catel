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
    using System.Runtime.InteropServices;
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
            Argument.IsNotNull(() => viewManager);
            Argument.IsNotNull(() => saveFileService);

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
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <remarks>If <paramref name="exportMode" /> is <see cref="ExportMode.Print" /> then the <paramref name="dpiX" /> and <paramref name="dpiY" /> argument will be ignored.</remarks>
        public virtual void Export(IViewModel viewModel, ExportMode exportMode = ExportMode.Print, double dpiX = 96, double dpiY = 96)
        {
            Argument.IsNotNull(() => viewModel);

            var view = _viewManager.GetViewsOfViewModel(viewModel).OfType<UIElement>().FirstOrDefault();
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
#if !SILVERLIGHT 
                if (exportMode == ExportMode.File)
                {
                    SaveToFile(bitmap);
                }
                else
                {
                    Clipboard.SetImage(bitmap);
                }
#else
                SaveToFile(bitmap);
#endif
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
#if SILVERLIGHT
            _saveFileService.Filter = "BMP (*.bmp) |*.bmp";
            using (var stream = _saveFileService.DetermineFile())
            {
                if (stream != null)
                {
                    var writeableBitmap = new WriteableBitmap(bitmap);
                    var bitmapByteArray = ConvertWritableBitmapToByteArray(writeableBitmap);
                    stream.Write(bitmapByteArray, 0, bitmapByteArray.Length);
                }
            }
#else
            _saveFileService.Filter = "PNG (*.png) |*.png";
            if (_saveFileService.DetermineFile())
            {
                string fileName = _saveFileService.FileName;
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

        /// <summary>
        /// Serializes an integer into a byte array
        /// </summary>
        /// <param name="value">
        ///     The value
        /// </param>
        /// <param name="array">
        ///     The byte array
        /// </param>
        /// <param name="offset">
        ///     The offset
        /// </param>
        /// <remarks>
        ///   TODO: Create an extension method
        /// </remarks>
        /// <exception cref="ArgumentException">If <paramref name="array"/> is <c>null</c> or an empty array</exception>
        private static void SerializeIntoByteArray(int value, byte[] array, int offset)
        {
            array[offset] = (byte)value;
            array[offset + 1] = (byte)(value >> 8);
            array[offset + 2] = (byte)(value >> 16);
            array[offset + 3] = (byte)(value >> 24);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        /// <remarks>
        ///   TODO: Create an extension method
        /// </remarks>
        private static byte[] ConvertWritableBitmapToByteArray(WriteableBitmap bmp)
        {
            var result = new byte[56 + 4 * bmp.PixelWidth * bmp.PixelHeight];

            // bitmap signature
            result[0] = (byte)'B';
            result[1] = (byte)'M';

            // offset
            result[10] = 56;

            // header size
            result[14] = 40;

            // width of the image
            SerializeIntoByteArray(bmp.PixelWidth, result, 18);

            // width of the height
            SerializeIntoByteArray(bmp.PixelHeight, result, 22);

            // planes
            result[26] = 1;

            // bits per pixel
            result[28] = 32;

            // bitmap raw size, 4 bytes per pixel
            var size = bmp.PixelHeight * bmp.PixelHeight * 4;
            SerializeIntoByteArray(size, result, 34);

            int idx = result.Length - 1;
            for (int i = 0; i < bmp.PixelHeight; i++)
            {
                for (int j = 0; j < bmp.PixelWidth; j++)
                {
                    var color = bmp.Pixels[i * bmp.PixelWidth + (bmp.PixelWidth - j - 1)];
                    SerializeIntoByteArray(color, result, idx - 3);
                    idx -= 4;
                }
            }

            return result;
        }
#endif
        #endregion
    }
}