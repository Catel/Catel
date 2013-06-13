// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    /// <summary>
    /// Test data class for the <see cref="ICameraService"/>.
    /// </summary>
    public class CameraServiceTestData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraServiceTestData"/> class.
        /// </summary>
        public CameraServiceTestData()
        {
            AvailableResolutions = new List<Size>();
            AvailableResolutions.Add(new Size(640, 960)); // TODO: What resolutions are normal?

            SupportedFlashModes = FlashMode.On | FlashMode.Off | FlashMode.Auto | FlashMode.RedEyeReduction;
            SupportedCameraTypes = CameraType.Primary | CameraType.FrontFacing;
            IsFocusAtPointSupported = false;
            IsFocusSupported = false;
            Orientation = 0d;
            PreviewResolution = new Size(320, 480);
            Resolution = AvailableResolutions[0];
        }

        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        /// <value>The available resolutions.</value>
        public List<Size> AvailableResolutions { get; set; }

        /// <summary>
        /// Gets or sets the supported flash modes.
        /// </summary>
        /// <value>The supported flash modes.</value>
        public FlashMode SupportedFlashModes { get; set; }

        /// <summary>
        /// Gets or sets the supported camera types.
        /// </summary>
        /// <value>The supported camera types.</value>
        public CameraType SupportedCameraTypes { get; set; }

        /// <summary>
        /// Gets a value indicating whether the camera can programmatically auto focus on a specific point in the viewfinder.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the camera can programmatically auto focus on a specific point in the viewfinder; otherwise, <c>false</c>.
        /// </value>
        public bool IsFocusAtPointSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether the camera can be auto-focused programmatically.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the camera can be auto-focused programmatically; otherwise, <c>false</c>.
        /// </value>
        public bool IsFocusSupported { get; set; }

        /// <summary>
        /// Gets the number of degrees that the viewfind brush needs to be rotated clockwise to align with the camera sensor.
        /// </summary>
        /// <value>The number of degrees that the viewfinder brush needs to be rotated clockwise to align with the camera sensor.</value>
        public double Orientation { get; set; }

        /// <summary>
        /// Gets the preview resolution of the images.
        /// </summary>
        /// <value>The preview resolution.</value>
        public Size PreviewResolution { get; set; }

        /// <summary>
        /// Gets or sets the resolution of the actual images.
        /// </summary>
        /// <value>The resolution.</value>
        public Size Resolution { get; set; }
    }

    /// <summary>
    /// Test implementation of the <see cref="ICameraService"/>.
    /// </summary>
    public class CameraService : CameraServiceBase
    {
        #region Fields
        private CameraServiceTestData _testData = new CameraServiceTestData();
        private readonly WriteableBitmap _testImage;
        private readonly DispatcherTimer _timer;

        private bool _isFocusing;
        private FlashMode _flashMode;
        private CameraType _cameraType;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraService"/> class.
        /// </summary>
        /// <param name="testImage">The test image to use.</param>
        /// <param name="updateDelayInMilliseconds">The update delay in milliseconds. Must be a value between 25 and 1000.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="testImage"/> is <c>null</c>.</exception>
        public CameraService(BitmapImage testImage, int updateDelayInMilliseconds = 50)
        {
            Argument.IsNotNull("testImage", testImage);
            Argument.IsNotOutOfRange("updateDelayInMilliseconds", updateDelayInMilliseconds, 25, 1000);

            _testImage = new WriteableBitmap(testImage);

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, updateDelayInMilliseconds);
            _timer.Tick += OnTimerTick;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is focusing.
        /// </summary>
        public bool IsFocusing { get { return _isFocusing;} }

        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        /// <returns>The available resolutions.</returns>
        protected override IEnumerable<Size> GetAvailableResolutions()
        {
            return _testData.AvailableResolutions;
        }

        /// <summary>
        /// Gets the flash mode.
        /// </summary>
        /// <returns>The flash mode.</returns>
        protected override FlashMode GetFlashMode()
        {
            return _flashMode;
        }

        /// <summary>
        /// Sets the flash mode.
        /// </summary>
        /// <param name="flashMode">The flash mode.</param>
        protected override void SetFlashMode(FlashMode flashMode)
        {
            if (Enum<FlashMode>.Flags.IsFlagSet(_testData.SupportedFlashModes, flashMode))
            {
                // Silently go to off (according to documentation)
                _flashMode = FlashMode.Off;
            }

            _flashMode = flashMode;
        }

        /// <summary>
        /// Gets the type of the camera.
        /// </summary>
        /// <returns>The camera type.</returns>
        protected override CameraType GetCameraType()
        {
            return _cameraType;
        }

        /// <summary>
        /// Gets a value indicating whether the camera can programmatically auto focus on a specific point in the viewfinder.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the camera can programmatically auto focus on a specific point in the viewfinder; otherwise, <c>false</c>.
        /// </returns>
        protected override bool GetIsFocusAtPointSupported()
        {
            return _testData.IsFocusAtPointSupported;
        }

        /// <summary>
        /// Gets a value indicating whether the camera can be auto-focused programmatically.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the camera can be auto-focused programmatically; otherwise, <c>false</c>.
        /// </returns>
        protected override bool GetIsFocusSupported()
        {
            return _testData.IsFocusSupported;
        }

        /// <summary>
        /// Gets the number of degrees that the viewfind brush needs to be rotated clockwise to align with the camera sensor.
        /// </summary>
        /// <returns>The orientation.</returns>
        protected override double GetOrientation()
        {
            return _testData.Orientation;
        }

        /// <summary>
        /// Gets the preview resolution of the images.
        /// </summary>
        /// <returns>The preview resolution.</returns>
        protected override Size GetPreviewResolution()
        {
            return _testData.PreviewResolution;
        }

        /// <summary>
        /// Gets the resolution of the actual images.
        /// </summary>
        /// <returns>The resolution.</returns>
        protected override Size GetResolution()
        {
            return _testData.Resolution;
        }

        /// <summary>
        /// Sets the resolution of the actual images.
        /// </summary>
        /// <param name="resolution">The resolution.</param>
        protected override void SetResolution(Size resolution)
        {
            _testData.Resolution = resolution;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when the 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (_testImage == null)
            {
                return;
            }

            // Move image from left to right (strip all right pixels and paste them at the left)
            int imageWidth = _testImage.PixelWidth;
            int imageHeight = _testImage.PixelHeight;
            int[] lastPixelColumn = new int[imageHeight];

            // Copy last column into a buffer
            for (int i = 0; i < imageHeight; i++)
            {
                int rightTopPixel = imageWidth - 1;
                lastPixelColumn[i] = _testImage.Pixels[rightTopPixel + (i * imageWidth)];
            }

            // Now move all rows to the right, starting at the right
            for (int x = 1; x < imageHeight; x++)
            {
                // It looks like we are looping from left -> right, but we are using
                // the y value to subtract so actually we are moving from right -> left
                for (int y = 0; y < imageWidth; y++)
                {
                    int pixelIndex = ((x + 1) * imageWidth) - y - 1;
                    _testImage.Pixels[pixelIndex] = _testImage.Pixels[pixelIndex - 1];
                }
            }

            // Move the buffer to the first row
            for (int i = 0; i < imageHeight; i++)
            {
                int leftTopPixel = 0;
                _testImage.Pixels[leftTopPixel + (i * imageWidth)] = lastPixelColumn[i];
            }

            using (var stream = new MemoryStream())
            {
                _testImage.SaveJpeg(stream, (int) _testData.PreviewResolution.Width, (int) _testData.PreviewResolution.Height, 0, 100);
                RaiseCaptureThumbnailAvailable(new ContentReadyEventArgs(stream));
            }
        }

        /// <summary>
        /// Updates the test data.
        /// </summary>
        /// <param name="testData">The test data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="testData"/> is <c>null</c>.</exception>
        public void UpdateTestData(CameraServiceTestData testData)
        {
            Argument.IsNotNull("testData", testData);

            _testData = testData;
        }

        /// <summary>
        /// Starts the camera service so it's retrieving data.
        /// </summary>
        /// <param name="cameraType">Type of the camera.</param>
        /// <remarks>
        /// This method is already protected and only called when the service is currently not running.
        /// </remarks>
        protected override void StartService(CameraType cameraType)
        {
            _cameraType = cameraType;

            _timer.Start();
        }

        /// <summary>
        /// Stops the camera service so it's no longer retrieving data.
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void StopService()
        {
            _isFocusing = false;

            _timer.Stop();
        }

        /// <summary>
        /// Starts a camera auto focus operation.
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void FocusCamera()
        {
            _isFocusing = true;
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
            _isFocusing = true;
        }

        /// <summary>
        /// Cancels the current camera auto focus operation.
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void CancelCameraFocus()
        {
            _isFocusing = false;
        }

        /// <summary>
        /// Initiates a full-resolution capture of the current image displayed in the viewfinder
        /// </summary>
        /// <remarks>
        /// This method is already protected and only called when the service is currently running.
        /// </remarks>
        protected override void CaptureCameraImage()
        {
            RaiseCaptureStarted(EventArgs.Empty);

            using (var stream = new MemoryStream())
            {
                _testImage.SaveJpeg(stream, (int)_testData.Resolution.Width, (int)_testData.Resolution.Height, 0, 100);
                RaiseCaptureImageAvailable(new ContentReadyEventArgs(stream));
            }

            RaiseCaptureCompleted(new CameraOperationCompletedEventArgs(null));
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
            return Enum<CameraType>.Flags.IsFlagSet(_testData.SupportedCameraTypes, type);
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
            return Enum<FlashMode>.Flags.IsFlagSet(_testData.SupportedFlashModes, mode);
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
