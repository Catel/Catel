// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Base implementation of the <see cref="ICameraService"/>. This implementation takes care of some
    /// protection code to prevent double initialization or forgotten initialization.
    /// <para />
    /// This class ensures that the service is running when setting or getting cricical properties or
    /// calling critical methods. This might look a bit dumb in the first place, but saves the developer
    /// of the end-classes lots of redundant checking.
    /// </summary>
    public abstract class CameraServiceBase : ViewModelServiceBase, ICameraService
    {
        #region Fields
        private bool _isRunning;
        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        /// <value>The available resolutions.</value>
        public IEnumerable<Size> AvailableResolutions 
        { 
            get
            {
                EnsureServiceRunning();

                return GetAvailableResolutions();
            } 
        }

        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        /// <returns>The available resolutions.</returns>
        protected abstract IEnumerable<Size> GetAvailableResolutions();

        /// <summary>
        /// Gets or sets the flash mode. Unsupported flash modes silently default to <see cref="Catel.MVVM.Services.FlashMode.Off"/>.
        /// </summary>
        /// <value>The flash mode.</value>
        public FlashMode FlashMode
        {
            get
            {
                EnsureServiceRunning();

                return GetFlashMode();
            }
            set
            {
                EnsureServiceRunning();

                SetFlashMode(value);
            }
        }

        /// <summary>
        /// Gets the flash mode.
        /// </summary>
        /// <returns>The flash mode.</returns>
        protected abstract FlashMode GetFlashMode();

        /// <summary>
        /// Sets the flash mode.
        /// </summary>
        /// <param name="flashMode">The flash mode.</param>
        protected abstract void SetFlashMode(FlashMode flashMode);

        /// <summary>
        /// Gets the type of the camera.
        /// </summary>
        /// <value>The type of the camera.</value>
        public CameraType CameraType
        {
            get
            {
                EnsureServiceRunning();

                return GetCameraType();
            }
        }

        /// <summary>
        /// Gets the type of the camera.
        /// </summary>
        /// <returns>The camera type.</returns>
        protected abstract CameraType GetCameraType();

        /// <summary>
        /// Gets a value indicating whether the camera can programmatically auto focus on a specific point in the viewfinder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the camera can programmatically auto focus on a specific point in the viewfinder; otherwise, <c>false</c>.
        /// </value>
        public bool IsFocusAtPointSupported
        {
            get
            {
                EnsureServiceRunning();

                return GetIsFocusAtPointSupported();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the camera can programmatically auto focus on a specific point in the viewfinder.
        /// </summary>
        /// <returns><c>true</c> if the camera can programmatically auto focus on a specific point in the viewfinder; otherwise, <c>false</c>.</returns>
        protected abstract bool GetIsFocusAtPointSupported();

        /// <summary>
        /// Gets a value indicating whether the camera can be auto-focused programmatically.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the camera can be auto-focused programmatically; otherwise, <c>false</c>.
        /// </value>
        public bool IsFocusSupported
        {
            get
            {
                EnsureServiceRunning();

                return GetIsFocusSupported();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the camera can be auto-focused programmatically.
        /// </summary>
        /// <returns><c>true</c> if the camera can be auto-focused programmatically; otherwise, <c>false</c>.</returns>
        protected abstract bool GetIsFocusSupported();

        /// <summary>
        /// Gets the number of degrees that the viewfind brush needs to be rotated clockwise to align with the camera sensor.
        /// </summary>
        /// <value>The number of degrees that the viewfinder brush needs to be rotated clockwise to align with the camera sensor.</value>
        public double Orientation
        {
            get
            {
                EnsureServiceRunning();

                return GetOrientation();
            }
        }

        /// <summary>
        /// Gets the number of degrees that the viewfind brush needs to be rotated clockwise to align with the camera sensor.
        /// </summary>
        /// <returns>The orientation.</returns>
        protected abstract double GetOrientation();

        /// <summary>
        /// Gets the preview resolution of the images.
        /// </summary>
        /// <value>The preview resolution.</value>
        public Size PreviewResolution
        {
            get
            {
                EnsureServiceRunning();

                return GetPreviewResolution();
            }
        }

        /// <summary>
        /// Gets the preview resolution of the images.
        /// </summary>
        /// <returns>The preview resolution.</returns>
        protected abstract Size GetPreviewResolution();

        /// <summary>
        /// Gets or sets the resolution of the actual images.
        /// </summary>
        /// <value>The resolution.</value>
        public Size Resolution
        {
            get
            {
                EnsureServiceRunning();

                return GetResolution();
            }
            set
            {
                EnsureServiceRunning();

                SetResolution(value);
            }
        }

        /// <summary>
        /// Gets the resolution of the actual images.
        /// </summary>
        /// <returns>The resolution.</returns>
        protected abstract Size GetResolution();

        /// <summary>
        /// Sets the resolution of the actual images.
        /// </summary>
        /// <param name="resolution">The resolution.</param>
        protected abstract void SetResolution(Size resolution);
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the camera has been initialized.
        /// </summary>
        public event EventHandler<CameraOperationCompletedEventArgs> Initialized;

        /// <summary>
        /// Occurs when the focus operation is completed.
        /// </summary>
        public event EventHandler<CameraOperationCompletedEventArgs> AutoFocusCompleted;

        /// <summary>
        /// Occurs when the capture sequence has started.
        /// </summary>
        public event EventHandler CaptureStarted;

        /// <summary>
        /// Occurs when a thumbnail image is available.
        /// </summary>
        public event EventHandler<ContentReadyEventArgs> CaptureThumbnailAvailable;

        /// <summary>
        /// Occurs when an image is available.
        /// </summary>
        public event EventHandler<ContentReadyEventArgs> CaptureImageAvailable;

        /// <summary>
        /// Occurs when the capture sequence is complete.
        /// </summary>
        public event EventHandler<CameraOperationCompletedEventArgs> CaptureCompleted;
        #endregion

        #region Methods
        /// <summary>
        /// Ensures that the service is running. If not, this method will throw a
        /// <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">The service is not running.</exception>
        internal void EnsureServiceRunning()
        {
            if (!_isRunning)
            {
                throw new NotSupportedException("The service must be started before this functionality can be used");
            }
        }

        /// <summary>
        /// Starts the camera service for the primary camera so it's retrieving data.
        /// </summary>
        public void Start()
        {
            Start(CameraType.Primary);
        }

        /// <summary>
        /// Starts the camera service for a specific camera type so it's retrieving data.
        /// </summary>
        /// <param name="cameraType">Type of the camera.</param>
        public void Start(CameraType cameraType)
        {
            if (_isRunning)
            {
                return;
            }

            if (!IsCameraTypeSupported(cameraType))
            {
                throw new NotSupportedException(string.Format("Camera type '{0}' is not supported by this camera", cameraType));
            }

            StartService(cameraType);

            _isRunning = true;
        }

        /// <summary>
        /// Starts the camera service so it's retrieving data.
        /// </summary>
        /// <param name="cameraType">Type of the camera.</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently not running.
        /// </remarks>
        protected abstract void StartService(CameraType cameraType);

        /// <summary>
        /// Stops the camera service so it's no longer retrieving data.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }

            StopService();

            _isRunning = false;
        }

        /// <summary>
        /// Stops the camera service so it's no longer retrieving data.
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected abstract void StopService();

        /// <summary>
        /// Starts a camera auto focus operation.
        /// </summary>
        public void Focus()
        {
            EnsureServiceRunning();

            FocusCamera();
        }

        /// <summary>
        /// Starts a camera auto focus operation.
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected abstract void FocusCamera();

        /// <summary>
        /// Starts a camera auto focus operation on a specific point in the viewfinder, for those devices that support it.
        /// </summary>
        /// <param name="x">The horizontal location in the viewfinder; a value between 0 (left) and 1.0 (right).</param>
        /// <param name="y">The vertical location in the viewfinder; a value between 0 (top) and 1.0 (bottom).</param>
        public void FocusAtPoint(double x, double y)
        {
            if (x < 0) x = 0;
            if (x > 1) x = 1;
            if (y < 0) y = 0;
            if (y > 1) y = 1;

            EnsureServiceRunning();

            FocusCameraAtPoint(x, y);
        }

        /// <summary>
        /// Starts a camera auto focus operation on a specific point in the viewfinder, for those devices that support it.
        /// </summary>
        /// <param name="x">The horizontal location in the viewfinder; a value between 0 (left) and 1.0 (right).</param>
        /// <param name="y">The vertical location in the viewfinder; a value between 0 (top) and 1.0 (bottom).</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected abstract void FocusCameraAtPoint(double x, double y);

        /// <summary>
        /// Cancels the current camera auto focus operation.
        /// </summary>
        public void CancelFocus()
        {
            EnsureServiceRunning();

            CancelCameraFocus();
        }

        /// <summary>
        /// Cancels the current camera auto focus operation.
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected abstract void CancelCameraFocus();

        /// <summary>
        /// Initiates a full-resolution capture of the current image displayed in the viewfinder
        /// </summary>
        public void CaptureImage()
        {
            EnsureServiceRunning();

            CaptureCameraImage();
        }

        /// <summary>
        /// Initiates a full-resolution capture of the current image displayed in the viewfinder
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected abstract void CaptureCameraImage();

        /// <summary>
        /// Determines whether a particular camera type is supported on the device.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if the specified camera type is supported; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsCameraTypeSupported(CameraType type);

        /// <summary>
        /// Determines whether a particular flash mode is supported on the device.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>
        /// 	<c>true</c> if the specified flash mode is supported; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFlashModeSupported(FlashMode mode)
        {
            EnsureServiceRunning();

            return IsFlashModeSupportedByCamera(mode);
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
        protected abstract bool IsFlashModeSupportedByCamera(FlashMode mode);

        /// <summary>
        /// Copies the current viewfinder ARGB frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The ARGB pixel data.</param>
        public void GetPreviewBufferArgb32(int[] pixelData)
        {
            EnsureServiceRunning();

            GetPreviewBufferArgb32FromCamera(pixelData);
        }

        /// <summary>
        /// Copies the current viewfinder ARGB frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The ARGB pixel data.</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected abstract void GetPreviewBufferArgb32FromCamera(int[] pixelData);

        /// <summary>
        /// Copies the luminance data for the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The YCrCb pixel data.</param>
        public void GetPreviewBufferY(byte[] pixelData)
        {
            EnsureServiceRunning();

            GetPreviewBufferYFromCamera(pixelData);
        }

        /// <summary>
        /// Copies the luminance data for the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The YCrCb pixel data.</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected abstract void GetPreviewBufferYFromCamera(byte[] pixelData);

        /// <summary>
        /// Copies the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The pixel data.</param>
        public void GetPreviewBufferYCbCr(byte[] pixelData)
        {
            EnsureServiceRunning();

            GetPreviewBufferYCbCrFromCamera(pixelData);
        }

        /// <summary>
        /// Copies the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The pixel data.</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected abstract void GetPreviewBufferYCbCrFromCamera(byte[] pixelData);

        /// <summary>
        /// Raises the <see cref="Initialized"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Catel.MVVM.Services.CameraOperationCompletedEventArgs"/> instance containing the event data.</param>
        protected void RaiseInitialized(CameraOperationCompletedEventArgs e)
        {
            Initialized.SafeInvoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="AutoFocusCompleted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Catel.MVVM.Services.CameraOperationCompletedEventArgs"/> instance containing the event data.</param>
        protected void RaiseAutoFocusCompleted(CameraOperationCompletedEventArgs e)
        {
            AutoFocusCompleted.SafeInvoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="CaptureStarted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void RaiseCaptureStarted(EventArgs e)
        {
            CaptureStarted.SafeInvoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="CaptureThumbnailAvailable"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Catel.MVVM.Services.ContentReadyEventArgs"/> instance containing the event data.</param>
        protected void RaiseCaptureThumbnailAvailable(ContentReadyEventArgs e)
        {
            CaptureThumbnailAvailable.SafeInvoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="CaptureImageAvailable"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Catel.MVVM.Services.ContentReadyEventArgs"/> instance containing the event data.</param>
        protected void RaiseCaptureImageAvailable(ContentReadyEventArgs e)
        {
            CaptureImageAvailable.SafeInvoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="CaptureCompleted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Catel.MVVM.Services.CameraOperationCompletedEventArgs"/> instance containing the event data.</param>
        protected void RaiseCaptureCompleted(CameraOperationCompletedEventArgs e)
        {
            CaptureCompleted.SafeInvoke(this, e);
        }
        #endregion
    }
}
