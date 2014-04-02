// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICameraService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 3021 // 'type' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    
#if XAMARIN
    using System.Drawing;
#elif NETFX_CORE
    using global::Windows.Foundation;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Defines the operating mode of the camera flash.
    /// </summary>
    [Flags]
    public enum FlashMode
    {
        /// <summary>
        /// The camera flash is enabled.
        /// </summary>
        On = 1,

        /// <summary>
        /// The camera flash is disabled.
        /// </summary>
        Off = 2,

        /// <summary>
        /// The camera flash is in auto mode.
        /// </summary>
        Auto = 4,

        /// <summary>
        /// The camera flash is in red-eye reduction mode.
        /// </summary>
        RedEyeReduction = 8
    }

    /// <summary>
    /// Specifies the general location of the camera on the device.
    /// </summary>
    [Flags]
    public enum CameraType
    {
        /// <summary>
        /// The camera is located on the back of the device.
        /// </summary>
        Primary = 1,

        /// <summary>
        /// The camera is located on the front of the device.
        /// </summary>
        FrontFacing = 2
    }

    /// <summary>
    /// Interface for retrieving the camera information.
    /// </summary>
    [CLSCompliant(false)]
    public interface ICameraService
    {
        #region Properties
        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        /// <value>The available resolutions.</value>
        IEnumerable<Size> AvailableResolutions { get; }

        /// <summary>
        /// Gets or sets the flash mode. Unsupported flash modes silently default to <see cref="Catel.Services.FlashMode.Off"/>.
        /// </summary>
        /// <value>The flash mode.</value>
        FlashMode FlashMode { get; set; }

        /// <summary>
        /// Gets the type of the camera.
        /// </summary>
        /// <value>The type of the camera.</value>
        CameraType CameraType { get; }

        /// <summary>
        /// Gets a value indicating whether the camera can programmatically auto focus on a specific point in the viewfinder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the camera can programmatically auto focus on a specific point in the viewfinder; otherwise, <c>false</c>.
        /// </value>
        bool IsFocusAtPointSupported { get; }

        /// <summary>
        /// Gets a value indicating whether the camera can be auto-focused programmatically.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the camera can be auto-focused programmatically; otherwise, <c>false</c>.
        /// </value>
        bool IsFocusSupported { get;}

        /// <summary>
        /// Gets the number of degrees that the viewfind brush needs to be rotated clockwise to align with the camera sensor.
        /// </summary>
        /// <value>The number of degrees that the viewfinder brush needs to be rotated clockwise to align with the camera sensor.</value>
        double Orientation { get; }

        /// <summary>
        /// Gets the preview resolution of the images.
        /// </summary>
        /// <value>The preview resolution.</value>
        [CLSCompliant(false)]
        Size PreviewResolution { get; }

        /// <summary>
        /// Gets or sets the resolution of the actual images.
        /// </summary>
        /// <value>The resolution.</value>
        [CLSCompliant(false)]
        Size Resolution { get; set; }

        ///// <summary>
        ///// Gets the YCbCr pixel layout of the camera preview buffer.
        ///// </summary>
        ///// <value>The YCbCr pixel layout of the camera.</value>
        //YCbCrPixelLayout YCbCrPixelLayout { get; }
        // TODO: Support YCbCr pixel layout
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the camera has been initialized.
        /// </summary>
        event EventHandler<CameraOperationCompletedEventArgs> Initialized;

        /// <summary>
        /// Occurs when the focus operation is completed.
        /// </summary>
        event EventHandler<CameraOperationCompletedEventArgs> AutoFocusCompleted;

        /// <summary>
        /// Occurs when the capture sequence has started.
        /// </summary>
        event EventHandler CaptureStarted;

        /// <summary>
        /// Occurs when a thumbnail image is available.
        /// </summary>
        event EventHandler<ContentReadyEventArgs> CaptureThumbnailAvailable;

        /// <summary>
        /// Occurs when an image is available.
        /// </summary>
        event EventHandler<ContentReadyEventArgs> CaptureImageAvailable;

        /// <summary>
        /// Occurs when the capture sequence is complete.
        /// </summary>
        event EventHandler<CameraOperationCompletedEventArgs> CaptureCompleted;
        #endregion

        #region Methods
        /// <summary>
        /// Starts the camera service so it's retrieving data.
        /// </summary>
        void Start();

        /// <summary>
        /// Starts the camera service for a specific camera type so it's retrieving data.
        /// </summary>
        /// <param name="cameraType">Type of the camera.</param>
        void Start(CameraType cameraType);

        /// <summary>
        /// Stops the camera service so it's no longer retrieving data.
        /// </summary>
        void Stop();

        /// <summary>
        /// Starts a camera auto focus operation.
        /// </summary>
        void Focus();

        /// <summary>
        /// Starts a camera auto focus operation on a specific point in the viewfinder, for those devices that support it.
        /// </summary>
        /// <param name="x">The horizontal location in the viewfinder; a value between 0 (left) and 1.0 (right).</param>
        /// <param name="y">The vertical location in the viewfinder; a value between 0 (top) and 1.0 (bottom).</param>
        void FocusAtPoint(double x, double y);

        /// <summary>
        /// Cancels the current camera auto focus operation.
        /// </summary>
        void CancelFocus();

        /// <summary>
        /// Initiates a full-resolution capture of the current image displayed in the viewfinder
        /// </summary>
        void CaptureImage();

        /// <summary>
        /// Determines whether a particular camera type is supported on the device.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the specified camera type is supported; otherwise, <c>false</c>.
        /// </returns>
        bool IsCameraTypeSupported(CameraType type);

        /// <summary>
        /// Determines whether a particular flash mode is supported on the device.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>
        /// 	<c>true</c> if the specified flash mode is supported; otherwise, <c>false</c>.
        /// </returns>
        bool IsFlashModeSupported(FlashMode mode);

        /// <summary>
        /// Copies the current viewfinder ARGB frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The ARGB pixel data.</param>
        void GetPreviewBufferArgb32(int[] pixelData);

        /// <summary>
        /// Copies the luminance data for the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The YCrCb pixel data.</param>
        void GetPreviewBufferY(byte[] pixelData);

        /// <summary>
        /// Copies the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The pixel data.</param>
        void GetPreviewBufferYCbCr(byte[] pixelData);
        #endregion
    }
}
