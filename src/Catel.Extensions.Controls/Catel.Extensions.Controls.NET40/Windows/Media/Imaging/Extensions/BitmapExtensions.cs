// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Media.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Extension methods for <see cref="BitmapSource"/>.
    /// </summary>
    public static class ImagingExtensions
    {
        /// <summary>
        /// Converts a <see cref="Bitmap"/> to a <see cref="BitmapSource"/> object.
        /// </summary>
        /// <param name="bitmap"><see cref="Bitmap"/> to convert.</param>
        /// <returns><see cref="BitmapSource"/> or null if an error occurs.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="bitmap"/> is <c>null</c>.</exception>
        public static BitmapSource ConvertBitmapToBitmapSource(this Bitmap bitmap)
        {
            Argument.IsNotNull("bitmap", bitmap);

            BitmapSource bitmapSource;

            try
            {
                bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                bitmapSource = null;
            }

            return bitmapSource;
        }

		/// <summary>
		/// Resizes the specified bitmap. It resizes the bitmap, but keeps the scale.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		/// <param name="maxWidth">Width of the max.</param>
		/// <param name="maxHeight">Height of the max.</param>
		/// <returns>
		/// 	<see cref="BitmapImage"/> with the right size.
		/// </returns>
		/// <remarks>If the original image is 1000x100 px and you specify a new dimension of 100x100 px the resized image is 100x10px</remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="bitmap"/> is <c>null</c>.</exception>
		public static BitmapImage Resize(this BitmapImage bitmap, int maxWidth, int maxHeight)
		{
            Argument.IsNotNull("bitmap", bitmap);

			var originalBitmap = bitmap.ConvertToImage();

			double height = bitmap.Height;
			double width = bitmap.Width;

			double heightResize = Convert.ToDouble(height) / Convert.ToDouble(maxHeight);
			double widthResize = Convert.ToDouble(width) / Convert.ToDouble(maxWidth);
			int newHeight = 0, newWidth = 0;

			if (heightResize > widthResize)
			{
				// Resize by height
				newHeight = Convert.ToInt32(Convert.ToDouble(height) / heightResize);
				newWidth = Convert.ToInt32(Convert.ToDouble(width) / heightResize);
			}
			else
			{
				// Resize by width
				newHeight = Convert.ToInt32(height / widthResize);
				newWidth = Convert.ToInt32(width / widthResize);
			}

			Image newBitmap = new Bitmap(newWidth, newHeight);
			Graphics graphics = Graphics.FromImage(newBitmap);
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			graphics.CompositingQuality = CompositingQuality.HighQuality;

			graphics.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);
			graphics.Dispose();
			
			BitmapImage result = newBitmap.ConvertToBitmapImage();

			originalBitmap.Dispose();
			newBitmap.Dispose();

            // Force garbage collection to prevent lots of memory usage
			GC.Collect();			

			return result;
		}

		/// <summary>
		/// Converts to image.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		/// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="bitmap"/> is <c>null</c>.</exception>
		public static Image ConvertToImage(this BitmapImage bitmap)
		{
            Argument.IsNotNull("bitmap", bitmap);

			Image image;

			using (MemoryStream memoryStream = new MemoryStream())
			{							
				BmpBitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
				bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
				bitmapEncoder.Save(memoryStream);
				
				image = Image.FromStream(memoryStream);			
			}

            // Force garbage collection to prevent lots of memory usage
			GC.Collect();			

			return image; 
		}

		/// <summary>
		/// Converts to bitmap image.
		/// </summary>
		/// <param name="image">The image.</param>
		/// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="image"/> is <c>null</c>.</exception>
		public static BitmapImage ConvertToBitmapImage(this Image image)
		{
            Argument.IsNotNull("image", image);

			BitmapImage bitmapImage = new BitmapImage();

			bitmapImage.BeginInit();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				image.Save(memoryStream, ImageFormat.Jpeg);
				bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
			}
			bitmapImage.EndInit();

			return bitmapImage;
		}
	}    
}
