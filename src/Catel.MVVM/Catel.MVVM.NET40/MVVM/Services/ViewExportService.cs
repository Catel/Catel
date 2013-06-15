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
#if SILVERLIGHT 
        /// <summary>
        ///     The header bitmap.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct HeaderBitmap
        {
            /// <summary>
            ///     The bf type 1.
            /// </summary>
            [FieldOffset(0)]
            public byte bfType1;

            /// <summary>
            ///     The bf type 2.
            /// </summary>
            [FieldOffset(1)]
            public byte bfType2;

            /// <summary>
            ///     The bf size.
            /// </summary>
            [FieldOffset(2)]
            public int bfSize;

            /// <summary>
            ///     The bf reserved 1.
            /// </summary>
            [FieldOffset(6)]
            public short bfReserved1;

            /// <summary>
            ///     The bf reserved 2.
            /// </summary>
            [FieldOffset(8)]
            public short bfReserved2;

            /// <summary>
            ///     The bf offbits.
            /// </summary>
            [FieldOffset(12)]
            public int bfOffbits;

            /// <summary>
            ///     The bi size.
            /// </summary>
            [FieldOffset(16)]
            public int biSize;

            /// <summary>
            ///     The bi with.
            /// </summary>
            [FieldOffset(20)]
            public int biWith;

            /// <summary>
            ///     The bi height.
            /// </summary>
            [FieldOffset(24)]
            public int biHeight;

            /// <summary>
            ///     The bi planes.
            /// </summary>
            [FieldOffset(26)]
            public short biPlanes;

            /// <summary>
            ///     The bi bit count.
            /// </summary>
            [FieldOffset(28)]
            public short biBitCount;

            /// <summary>
            ///     The bi compression.
            /// </summary>
            [FieldOffset(32)]
            public int biCompression;

            /// <summary>
            ///     The bi size image.
            /// </summary>
            [FieldOffset(36)]
            public int biSizeImage;

            /// <summary>
            ///     The bi x pels per meter.
            /// </summary>
            [FieldOffset(40)]
            public int biXPelsPerMeter;

            /// <summary>
            ///     The bi y pels per meter.
            /// </summary>
            [FieldOffset(44)]
            public int biYPelsPerMeter;

            /// <summary>
            ///     The bi clr used.
            /// </summary>
            [FieldOffset(48)]
            public int biClrUsed;

            /// <summary>
            ///     The bi clr important.
            /// </summary>
            [FieldOffset(52)]
            public int biClrImportant;
        }
#endif
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
            var saveFileService = GetService<ISaveFileService>();

#if SILVERLIGHT
            saveFileService.Filter = "BMP (*.bmp) |*.bmp";
            using (var stream = saveFileService.DetermineFile())
            {
                if (stream != null)
                {
                    var writeableBitmap = new WriteableBitmap(bitmap);
                    var headerBitmap = new HeaderBitmap
                                           {
                                               bfType1 = 0x42,
                                               bfType2 = 0x4D, 
                                               biPlanes = 1, 
                                               bfOffbits = 0x2621440, 
                                               biBitCount = 24, 
                                               biHeight = writeableBitmap.PixelHeight, 
                                               biWith = writeableBitmap.PixelWidth, 
                                           };

                    var bitmapHeaderArray = RawSerialize(headerBitmap);
                    stream.Write(bitmapHeaderArray, 0, bitmapHeaderArray.Length);

                    var byteArray = ConvertWritableBitmapToByteArray(writeableBitmap);
                    stream.Write(byteArray, 0, byteArray.Length);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anything"></param>
        /// <returns></returns>
        public static byte[] RawSerialize<T>(T anything)
        {
            int rawsize = Marshal.SizeOf(typeof(T));
            var rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(anything, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return rawdata;
        }
        
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