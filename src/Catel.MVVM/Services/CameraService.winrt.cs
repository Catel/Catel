// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraService.wpf.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



#if UWP

namespace Catel.Services
{
    using System.Collections.Generic;

    public partial class CameraService
    {
        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        /// <returns>The available resolutions.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override IEnumerable<Size> GetAvailableResolutions()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Gets the flash mode.
        /// </summary>
        /// <returns>The flash mode.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override FlashMode GetFlashMode()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Sets the flash mode.
        /// </summary>
        /// <param name="flashMode">The flash mode.</param>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override void SetFlashMode(FlashMode flashMode)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Gets the type of the camera.
        /// </summary>
        /// <returns>The camera type.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override CameraType GetCameraType()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether the camera can programmatically auto focus on a specific point in the viewfinder.
        /// </summary>
        /// <returns><c>true</c> if the camera can programmatically auto focus on a specific point in the viewfinder; otherwise, <c>false</c>.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override bool GetIsFocusAtPointSupported()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether the camera can be auto-focused programmatically.
        /// </summary>
        /// <returns><c>true</c> if the camera can be auto-focused programmatically; otherwise, <c>false</c>.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override bool GetIsFocusSupported()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Gets the number of degrees that the viewfind brush needs to be rotated clockwise to align with the camera sensor.
        /// </summary>
        /// <returns>The orientation.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override double GetOrientation()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Gets the preview resolution of the images.
        /// </summary>
        /// <returns>The preview resolution.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override Size GetPreviewResolution()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Gets the resolution of the actual images.
        /// </summary>
        /// <returns>The resolution.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override Size GetResolution()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Sets the resolution of the actual images.
        /// </summary>
        /// <param name="resolution">The resolution.</param>
        /// <exception cref="MustBeImplementedException"></exception>
        protected override void SetResolution(Size resolution)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Starts the camera service so it's retrieving data.
        /// </summary>
        /// <param name="cameraType">Type of the camera.</param>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently not running.</remarks>
        protected override void StartService(CameraType cameraType)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Stops the camera service so it's no longer retrieving data.
        /// </summary>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently running.</remarks>
        protected override void StopService()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Starts a camera auto focus operation.
        /// </summary>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently running.</remarks>
        protected override void FocusCamera()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Starts a camera auto focus operation on a specific point in the viewfinder, for those devices that support it.
        /// </summary>
        /// <param name="x">The horizontal location in the viewfinder; a value between 0 (left) and 1.0 (right).</param>
        /// <param name="y">The vertical location in the viewfinder; a value between 0 (top) and 1.0 (bottom).</param>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently running.</remarks>
        protected override void FocusCameraAtPoint(double x, double y)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Cancels the current camera auto focus operation.
        /// </summary>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently running.</remarks>
        protected override void CancelCameraFocus()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Initiates a full-resolution capture of the current image displayed in the viewfinder
        /// </summary>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently running.</remarks>
        protected override void CaptureCameraImage()
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Determines whether a particular camera type is supported on the device.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified camera type is supported; otherwise, <c>false</c>.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        public override bool IsCameraTypeSupported(CameraType type)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Determines whether a particular flash mode is supported on the device.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns><c>true</c> if the specified flash mode is supported; otherwise, <c>false</c>.</returns>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently running.</remarks>
        protected override bool IsFlashModeSupportedByCamera(FlashMode mode)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Copies the current viewfinder ARGB frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The ARGB pixel data.</param>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently running.</remarks>
        protected override void GetPreviewBufferArgb32FromCamera(int[] pixelData)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Copies the luminance data for the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The YCrCb pixel data.</param>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently running.</remarks>
        protected override void GetPreviewBufferYFromCamera(byte[] pixelData)
        {
            throw new MustBeImplementedException();
        }

        /// <summary>
        /// Copies the current viewfinder frame into a buffer for further manipulation.
        /// </summary>
        /// <param name="pixelData">The pixel data.</param>
        /// <exception cref="MustBeImplementedException"></exception>
        /// <remarks>This method is already protected and only called when the service is currently running.</remarks>
        protected override void GetPreviewBufferYCbCrFromCamera(byte[] pixelData)
        {
            throw new MustBeImplementedException();
        }
    }
}

#endif
