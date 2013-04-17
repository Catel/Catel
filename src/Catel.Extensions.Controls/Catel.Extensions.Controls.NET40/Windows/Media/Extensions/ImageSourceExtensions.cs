// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSourceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Media
{
    using System;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Extensions for the <see cref="ImageSource"/> class.
    /// </summary>
    public static class ImageSourceExtensions
    {
        /// <summary>
        /// Converts an array of bytes to a <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <returns><see cref="ImageSource"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="bytes"/> is <c>null</c>.</exception>
        public static ImageSource ConvertByteArrayToImageSource(this byte[] bytes)
        {
            Argument.IsNotNull("bytes", bytes);

            if (bytes.Length == 0)
            {
                return null;
            }

            ImageSource result;

            // Create memory stream - it seems that if you clean up or dispose 
            // the memory stream, you cannot display the image any longer
            using (var memoryStream = new MemoryStream(bytes))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelWidth = 600;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                result = bitmapImage;
            }

            return result;
        }

        /// <summary>
        /// Converts an <see cref="ImageSource"/> to an array of bytes.
        /// </summary>
        /// <param name="image"><see creConvertImageSourceToByteArrayf="ImageSource"/> to convert.</param>
        /// <returns>Array of bytes.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="image"/> is <c>null</c>.</exception>
        public static byte[] ConvertImageSourceToByteArray(this ImageSource image)
        {
            Argument.IsNotNull("image", image);

            byte[] result;
            BitmapFrame bitmapFrame = null;

            var encoder = new JpegBitmapEncoder();
            var bitmapSource = (BitmapSource)image;

            bitmapFrame = BitmapFrame.Create(bitmapSource);

            encoder.Frames.Add(bitmapFrame);

            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);

                result = memoryStream.ToArray();

                encoder.Frames.Clear();
            }

            // Force garbage collection to prevent lots of memory usage
            GC.WaitForPendingFinalizers();
            GC.Collect();

            return result;
        }
    }
}
