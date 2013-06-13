// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimatedGif.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// <summary>
//   User control supporting animated gif.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using System.Windows.Resources;
    using System.Windows.Threading;

	/// <summary>
	/// User control supporting animated gif.
	/// </summary>
	public class AnimatedGif : System.Windows.Controls.Image
	{
		#region Win32 imports
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		private static extern IntPtr DeleteObject(IntPtr hDc);
		#endregion

		#region Fields
		private Bitmap _bitmap;
		#endregion

		#region Constructors
		#endregion

		#region Delegates
		/// <summary>
		/// OnFrameChanged delegate.
		/// </summary>
		private delegate void OnFrameChangedDelegate();
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether this instance is animating.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is animating; otherwise, <c>false</c>.
		/// </value>
		private bool IsAnimating { get; set; }

		/// <summary>
		/// Gets or sets the current frame.
		/// </summary>
		/// <value>The current frame.</value>
		private int CurrentFrame { get; set; }

		/// <summary>
		/// Gets or sets GifSource.
		/// </summary>
		/// <remarks>
		/// Wrapper for the GifSource dependency property.
		/// </remarks>
		public string GifSource
		{
			get { return (string)GetValue(GifSourceProperty); }
			set { SetValue(GifSourceProperty, value); }
		}

		/// <summary>
		/// DependencyProperty definition as the backing store for GifSource.
		/// </summary>
		public static readonly DependencyProperty GifSourceProperty = DependencyProperty.Register("GifSource", typeof(string),
			typeof(AnimatedGif), new UIPropertyMetadata(string.Empty, GifSource_Changed));
		#endregion

		#region Methods
		/// <summary>
		/// Invoked when the GifSource dependency property has changed.
		/// </summary>
		/// <param name="sender">The object that contains the dependency property.</param>
		/// <param name="e">The event data.</param>
		[DebuggerStepperBoundary()]
		private static void GifSource_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			AnimatedGif typedSender = sender as AnimatedGif;
			if (typedSender != null)
			{
				typedSender.SetImageGifSource();
			}
		}

		/// <summary>
		/// Sets the image gif source.
		/// </summary>
		private void SetImageGifSource()
		{
			if (_bitmap != null)
			{
				ImageAnimator.StopAnimate(_bitmap, OnFrameChanged);

				_bitmap = null;
			}

			if (string.IsNullOrEmpty(GifSource))
			{
				Source = null;

				InvalidateVisual();

				return;
			}

			// Check if this is a file
			if (File.Exists(GifSource))
			{
				_bitmap = (Bitmap)Image.FromFile(GifSource);
			}
			else
			{
				// Support looking for embedded resources
				Assembly assemblyToSearch = Assembly.GetAssembly(GetType());
				_bitmap = GetBitmapResourceFromAssembly(assemblyToSearch);
				if (_bitmap == null)
				{
					// Search calling assembly
					assemblyToSearch = Assembly.GetCallingAssembly();
					_bitmap = GetBitmapResourceFromAssembly(assemblyToSearch);
					if (_bitmap == null)
					{
						// Get entry assembly
						assemblyToSearch = Assembly.GetEntryAssembly();
						_bitmap = GetBitmapResourceFromAssembly(assemblyToSearch);
						if (_bitmap == null)
						{
                            throw new FileNotFoundException("Gif source was not found", GifSource);
						}
					}
				}
			}

			// Start animating
			ImageAnimator.Animate(_bitmap, OnFrameChanged);
		}

		/// <summary>
		/// Gets the bitmap resource from a specific assembly.
		/// </summary>
		/// <param name="assemblyToSearch">The assembly to search.</param>
		/// <returns><see cref="Bitmap"/> or null if resource is not found.</returns>
		[DebuggerStepperBoundary()]
		private Bitmap GetBitmapResourceFromAssembly(Assembly assemblyToSearch)
		{
			// Loop through all resources
			if (null != assemblyToSearch.FullName)
			{
				// Get stream resource info
				StreamResourceInfo streamResourceInfo = Application.GetResourceStream(new Uri(GifSource, UriKind.RelativeOrAbsolute));
				if (streamResourceInfo != null)
				{
					return (Bitmap)Image.FromStream(streamResourceInfo.Stream);
				}
			}

			return null;
		}

		/// <summary>
		/// Called when a frame has changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		[DebuggerStepperBoundary()]
		private void OnFrameChanged(object sender, EventArgs e)
		{
			// Dispatch the frame changed
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new OnFrameChangedDelegate(OnFrameChangedInMainThread));
		}

		/// <summary>
		/// Called when a frame changed in the main thread.
		/// </summary>
		[DebuggerStepperBoundary()]
		private void OnFrameChangedInMainThread()
		{
			// Update the frames
			ImageAnimator.UpdateFrames(_bitmap);

			// Get bitmap source
			Source = GetBitmapSource(_bitmap);

			// Invalidate visual
			InvalidateVisual();
		}

		/// <summary>
		/// Gets the bitmap source.
		/// </summary>
		/// <param name="gdiBitmap">The GDI bitmap.</param>
		/// <returns></returns>
		[DebuggerStepperBoundary()]
		private static BitmapSource GetBitmapSource(Bitmap gdiBitmap)
		{
			// Get the bitmap
			IntPtr hBitmap = gdiBitmap.GetHbitmap();

			// Create the bitmap
			BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

			// Delete bitmap pointer
			DeleteObject(hBitmap);

			// Return bitmap source
			return bitmapSource;
		}
		#endregion
	}
}
