// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using Logging;
    using Reflection;

    /// <summary>
    /// Extensions for the <see cref="Visual"/>
    /// </summary>
    public static class VisualExtensions
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly Delegate _onVisualLoadedDelegate = DelegateHelper.CreateDelegate(typeof(RoutedEventHandler), typeof(VisualExtensions), "OnVisualLoaded");
        #endregion

        /// <summary>
        /// Disables the hardware acceleration for the specified visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <remarks>
        /// When the visual is not yet loaded, this method tries to subscribe to the <c>Control.Loaded</c> event so disabled the 
        /// hardware acceleration as soon as the control is loaded.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="visual"/> is <c>null</c>.</exception>
        public static void DisableHardwareAcceleration(this Visual visual)
        {
            Argument.IsNotNull("visual", visual);

            var visualType = visual.GetType();
            var hwndSource = PresentationSource.FromVisual(visual) as HwndSource;
            if (hwndSource == null)
            {
                // Visual is now null, try to subscribe to the loaded event
                var loadedEvent = visualType.GetEventEx("Loaded");
                if (loadedEvent != null)
                {
                    Log.Debug("Hardware acceleration cannot yet be turned off for visual '{0}', but will be as soon as the visual is loaded", visualType);
                    
                    loadedEvent.AddEventHandler(visual, _onVisualLoadedDelegate);
                }
                else
                {
                    Log.Warning("Failed to disabled hardware acceleration because visual '{0}' does not yet have a handle", visualType);
                }
            }
            else
            {
                // Set to software only
                hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;

                Log.Debug("Disabled hardware acceleration for '{0}'", visualType);
            }
        }

        /// <summary>
        /// Called when a visual has been loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
// ReSharper disable UnusedMember.Local
        private static void OnVisualLoaded(object sender, RoutedEventArgs e)
// ReSharper restore UnusedMember.Local
        {
            var visual = (Visual)sender;

            var loadedEvent = visual.GetType().GetEventEx("Loaded");
            loadedEvent.RemoveEventHandler(visual, _onVisualLoadedDelegate);

            DisableHardwareAcceleration(visual);
        }
    }
}
