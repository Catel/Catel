// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

#if UAP
    using Windows.Foundation.Metadata;
#endif

    /// <summary>
    /// Device helper class.
    /// </summary>
    public static class DeviceHelper
    {
        private static DeviceType? _deviceType;

        /// <summary>
        /// Gets a value indicating whether this code runs on a desktop.
        /// </summary>
        /// <value><c>true</c> if this instance is desktop; otherwise, <c>false</c>.</value>
        public static bool IsDesktop
        {
            get { return GetDeviceType() == DeviceType.Desktop; }
        }

        /// <summary>
        /// Gets a value indicating whether this code runs on a tablet.
        /// </summary>
        /// <value><c>true</c> if this instance is tablet; otherwise, <c>false</c>.</value>
        public static bool IsTablet
        {
            get { return GetDeviceType() == DeviceType.Tablet; }
        }

        /// <summary>
        /// Gets a value indicating whether this code runs on a phone.
        /// </summary>
        /// <value><c>true</c> if this instance is phone; otherwise, <c>false</c>.</value>
        public static bool IsPhone
        {
            get { return GetDeviceType() == DeviceType.Phone; }
        }

        /// <summary>
        /// Gets a value indicating whether this code runs on an xbox.
        /// </summary>
        /// <value><c>true</c> if this instance is xbox; otherwise, <c>false</c>.</value>
        public static bool IsXbox
        {
            get { return GetDeviceType() == DeviceType.Xbox; }
        }

        /// <summary>
        /// Gets a value indicating whether this code runs on an Internet of Things (IoT) device.
        /// </summary>
        /// <value><c>true</c> if this instance is io t; otherwise, <c>false</c>.</value>
        public static bool IsIoT
        {
            get { return GetDeviceType() == DeviceType.IoT; }
        }

        /// <summary>
        /// Gets the type of the device.
        /// </summary>
        /// <returns>DeviceType.</returns>
        public static DeviceType GetDeviceType()
        {
            if (_deviceType.HasValue)
            {
                return _deviceType.Value;
            }

#if NET || SL5
            _deviceType = DeviceType.Desktop;
#elif WINDOWS_PHONE
            _deviceType = DeviceType.Phone;
#elif UAP
            var deviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (string.Equals(deviceFamily, "Windows.Mobile", StringComparison.OrdinalIgnoreCase))
            {
                _deviceType = DeviceType.Phone;
            }
            else if (string.Equals(deviceFamily, "Windows.Desktop", StringComparison.OrdinalIgnoreCase))
            {
                _deviceType = DeviceType.Desktop;
            }
            else
            {
                // TODO: add more support like xbox and IoT, but need devices for this
                _deviceType = DeviceType.Unknown;
            }

#elif IOS
            _deviceType = DeviceType.Unknown;
#elif ANDROID
            _deviceType = DeviceType.Unknown;
#endif

            return _deviceType.Value;
        }
    }
}