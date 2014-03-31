// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Devices;

    /// <summary>
    /// Official implementation of the <see cref="ICameraService"/>.
    /// </summary>
    public class CameraService : CameraServiceBase
    {
        #region Fields
        private PhotoCamera _photoCamera;
        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        /// <returns>The available resolutions.</returns>
        protected override IEnumerable<Size> GetAvailableResolutions()
        {
            var sizes = new List<Size>();

            foreach (var resolution in _photoCamera.AvailableResolutions)
            {
                sizes.Add(new Size(resolution.Width, resolution.Height));
            }

            return sizes;
        }

        /// <summary>
        /// Gets the flash mode.
        /// </summary>
        /// <returns>The flash mode.</returns>
        protected override FlashMode GetFlashMode()
        {
            return Enum<FlashMode>.ConvertFromOtherEnumValue(_photoCamera.FlashMode);
        }

        /// <summary>
        /// Sets the flash mode.
        /// </summary>
        /// <param name="flashMode">The flash mode.</param>
        protected override void SetFlashMode(FlashMode flashMode)
        {
            _photoCamera.FlashMode = Enum<Microsoft.Devices.FlashMode>.ConvertFromOtherEnumValue(flashMode);
        }

        /// <summary>
        /// Gets the type of the camera.
        /// </summary>
        /// <returns>The camera type.</returns>
        protected override CameraType GetCameraType()
        {
            return Enum<CameraType>.ConvertFromOtherEnumValue(_photoCamera.CameraType);
        }

        /// <summary>
        /// Gets a value indicating whether the camera can programmatically auto focus on a specific point in the viewfinder.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the camera can programmatically auto focus on a specific point in the viewfinder; otherwise, <c>false</c>.
        /// </returns>
        protected override bool GetIsFocusAtPointSupported()
        {
            return _photoCamera.IsFocusAtPointSupported;
        }

        /// <summary>
        /// Gets a value indicating whether the camera can be auto-focused programmatically.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the camera can be auto-focused programmatically; otherwise, <c>false</c>.
        /// </returns>
        protected override bool GetIsFocusSupported()
        {
            return _photoCamera.IsFocusSupported;
        }

        /// <summary>
        /// Gets the number of degrees that the viewfind brush needs to be rotated clockwise to align with the camera sensor.
        /// </summary>
        /// <returns>The orientation.</returns>
        protected override double GetOrientation()
        {
            return _photoCamera.Orientation;
        }

        /// <summary>
        /// Gets the preview resolution of the images.
        /// </summary>
        /// <returns>The preview resolution.</returns>
        protected override Size GetPreviewResolution()
        {
            var resolution = _photoCamera.PreviewResolution;
            return new Size(resolution.Width, resolution.Height);
        }

        /// <summary>
        /// Gets the resolution of the actual images.
        /// </summary>
        /// <returns>The resolution.</returns>
        protected override Size GetResolution()
        {
            var resolution = _photoCamera.Resolution;
            return new Size(resolution.Width, resolution.Height);
        }

        /// <summary>
        /// Sets the resolution of the actual images.
        /// </summary>
        /// <param name="resolution">The resolution.</param>
        protected override void SetResolution(Size resolution)
        {
            _photoCamera.Resolution = new System.Windows.Size(resolution.Width, resolution.Height);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the camera service so it's retrieving data.
        /// </summary>
        /// <param name="cameraType">Type of the camera.</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently not running.
        /// </remarks>
        protected override void StartService(CameraType cameraType)
        {
            _photoCamera = new PhotoCamera(Enum<Microsoft.Devices.CameraType>.ConvertFromOtherEnumValue(cameraType));

            _photoCamera.Initialized += OnPhotoCameraInitialized;
            _photoCamera.AutoFocusCompleted += OnPhotoCameraAutoFocusCompleted;
            _photoCamera.CaptureStarted += OnPhotoCameraCaptureStarted;
            _photoCamera.CaptureThumbnailAvailable += OnPhotoCameraCaptureThumbnailAvailable;
            _photoCamera.CaptureImageAvailable += OnPhotoCameraCaptureImageAvailable;
            _photoCamera.CaptureCompleted += OnPhotoCameraCaptureCompleted;
        }

        /// <summary>
        /// Stops the camera service so it's no longer retrieving data.
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void StopService()
        {
            _photoCamera.Initialized -= OnPhotoCameraInitialized;
            _photoCamera.AutoFocusCompleted -= OnPhotoCameraAutoFocusCompleted;
            _photoCamera.CaptureStarted -= OnPhotoCameraCaptureStarted;
            _photoCamera.CaptureThumbnailAvailable -= OnPhotoCameraCaptureThumbnailAvailable;
            _photoCamera.CaptureImageAvailable -= OnPhotoCameraCaptureImageAvailable;
            _photoCamera.CaptureCompleted -= OnPhotoCameraCaptureCompleted;

            _photoCamera.Dispose();
            _photoCamera = null;
        }

        /// <summary>
        /// Starts a camera auto focus operation.
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void FocusCamera()
        {
            _photoCamera.Focus();
        }

        /// <summary>
        /// Starts a camera auto focus operation on a specific point in the viewfinder, for those devices that support it.
        /// </summary>
        /// <param name="x">The horizontal location in the viewfinder; a value between 0 (left) and 1.0 (right).</param>
        /// <param name="y">The vertical location in the viewfinder; a value between 0 (top) and 1.0 (bottom).</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void FocusCameraAtPoint(double x, double y)
        {
            _photoCamera.FocusAtPoint(x, y);
        }

        /// <summary>
        /// Cancels the current camera auto focus operation.
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void CancelCameraFocus()
        {
            _photoCamera.CancelFocus();
        }

        /// <summary>
        /// Initiates a full-resolution capture of the current image displayed in the viewfinder
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void CaptureCameraImage()
        {
            _photoCamera.CaptureImage();
        }

        /// <summary>
        /// Determines whether a particular camera type is supported on the device.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the specified camera type is supported; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsCameraTypeSupported(CameraType type)
        {
            return PhotoCamera.IsCameraTypeSupported(Enum<Microsoft.Devices.CameraType>.ConvertFromOtherEnumValue(type));
        }

        /// <summary>
        /// Determines whether a particular flash mode is supported on the device.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>
        /// 	<c>true</c> if the specified flash mode is supported; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override bool IsFlashModeSupportedByCamera(FlashMode mode)
        {
            return _photoCamera.IsFlashModeSupported(Enum<Microsoft.Devices.FlashMode>.ConvertFromOtherEnumValue(mode));
        }

        /// <summary>
        /// Copies the current viewfinder ARGB frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The ARGB pixel data.</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void GetPreviewBufferArgb32FromCamera(int[] pixelData)
        {
            _photoCamera.GetPreviewBufferArgb32(pixelData);
        }

        /// <summary>
        /// Copies the luminance data for the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The YCrCb pixel data.</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void GetPreviewBufferYFromCamera(byte[] pixelData)
        {
            _photoCamera.GetPreviewBufferY(pixelData);
        }

        /// <summary>
        /// Copies the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The pixel data.</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void GetPreviewBufferYCbCrFromCamera(byte[] pixelData)
        {
            _photoCamera.GetPreviewBufferYCbCr(pixelData);
        }

        /// <summary>
        /// Called when the <see cref="Camera.Initialized"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Microsoft.Devices.CameraOperationCompletedEventArgs"/> instance containing the event data.</param>
        private void OnPhotoCameraInitialized(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            RaiseInitialized(new CameraOperationCompletedEventArgs(e.Exception));
        }

        /// <summary>
        /// Called when the <see cref="PhotoCamera.AutoFocusCompleted"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Microsoft.Devices.CameraOperationCompletedEventArgs"/> instance containing the event data.</param>
        private void OnPhotoCameraAutoFocusCompleted(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            RaiseAutoFocusCompleted(new CameraOperationCompletedEventArgs(e.Exception));
        }

        /// <summary>
        /// Called when the <see cref="PhotoCamera.CaptureStarted"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPhotoCameraCaptureStarted(object sender, EventArgs e)
        {
            RaiseCaptureStarted(e);
        }

        /// <summary>
        /// Called when the <see cref="PhotoCamera.CaptureThumbnailAvailable"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Microsoft.Devices.ContentReadyEventArgs"/> instance containing the event data.</param>
        private void OnPhotoCameraCaptureThumbnailAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            RaiseCaptureThumbnailAvailable(new ContentReadyEventArgs(e.ImageStream));
        }

        /// <summary>
        /// Called when the <see cref="PhotoCamera.CaptureImageAvailable"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Microsoft.Devices.ContentReadyEventArgs"/> instance containing the event data.</param>
        private void OnPhotoCameraCaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            RaiseCaptureImageAvailable(new ContentReadyEventArgs(e.ImageStream));
        }

        /// <summary>
        /// Called when the <see cref="PhotoCamera.CaptureCompleted"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Microsoft.Devices.CameraOperationCompletedEventArgs"/> instance containing the event data.</param>
        private void OnPhotoCameraCaptureCompleted(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            RaiseCaptureCompleted(new CameraOperationCompletedEventArgs(e.Exception));
        }
        #endregion
    }
}

#endif