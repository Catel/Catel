// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameRateCounter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// A counter to show the frame rate inside an application.
    /// </summary>
    public class FrameRateCounter : TextBlock
    {
        #region Fields
        private int _frameRateCounter;
        private readonly DispatcherTimer _frameRateTimer = new DispatcherTimer();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameRateCounter"/> class.
        /// </summary>
        public FrameRateCounter()
        {
            Loaded += OnControlLoaded;
            Unloaded += OnControlUnloaded;

            _frameRateTimer.Interval = new TimeSpan(0, 0, 0, 1);
            _frameRateTimer.Tick += (sender, e) => OnFrameRateCounterElapsed();
        }
        #endregion

        #region Methods
        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            _frameRateTimer.Start();

            CompositionTarget.Rendering += OnRendering;
        }

        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= OnRendering;

            _frameRateTimer.Stop();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            _frameRateCounter++;
        }

        private void OnFrameRateCounterElapsed()
        {
             Text = _frameRateCounter.ToString();
            _frameRateCounter = 0;
        }
        #endregion
    }
}