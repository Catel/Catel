// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrayscaleEffect.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Media.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media.Effects;

    /// <summary>
    /// Grayscale effect to convert objects to a grayscale mode.
    /// </summary>
    /// <remarks>
    /// This code is originally taken from http://bursjootech.blogspot.com/2008/06/grayscale-effect-pixel-shader-effect-in.html.
    /// <para />
    /// During the build of Catel, this pixel shader effect will not be recompiled to prevent all users to install the
    /// DirectX SDK. If you want to make changes, take a look at the readme of Catel.
    /// </remarks>
    public class GrayscaleEffect : ShaderEffectBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleEffect"/> class.
        /// </summary>
        public GrayscaleEffect()
        {
            if (!IsEnabled)
            {
                return;
            }

            UpdateShaderValue(DesaturationFactorProperty);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the desaturation factor.
        /// </summary>
        /// <value>The desaturation factor.</value>
        public double DesaturationFactor
        {
            get { return (double)GetValue(DesaturationFactorProperty); }
            set { SetValue(DesaturationFactorProperty, value); }
        }

        /// <summary>
        /// Property definition for <see cref="DesaturationFactor"/>.
        /// </summary>
        public static readonly DependencyProperty DesaturationFactorProperty = DependencyProperty.Register("DesaturationFactor", typeof(double),
#if SILVERLIGHT
            typeof(GrayscaleEffect), new PropertyMetadata(0.0, OnDesaturationFactorChanged));
#else
            typeof(GrayscaleEffect), new PropertyMetadata(0.0, OnDesaturationFactorChanged, CoerceDesaturationFactor));
#endif
        #endregion

        #region Methods
        /// <summary>
        /// Creates the pixel shader.
        /// </summary>
        /// <returns><see cref="PixelShader"/>.</returns>
        protected override PixelShader CreatePixelShader()
        {
            return new PixelShader { UriSource = new Uri(@"/Catel.Extensions.Controls;component/Windows/Media/Effects/GrayscaleEffect/GrayscaleEffect.ps", UriKind.RelativeOrAbsolute) };
        }

        /// <summary>
        /// Called when the <see cref="DesaturationFactor"/> property has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is required because Silverlight does not support coerce callback values for dependency properties.
        /// </remarks>
        private static void OnDesaturationFactorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
#if SILVERLIGHT
            PixelShaderConstantCallback((int)CoerceDesaturationFactor((DependencyObject)sender, e.NewValue));
#else
            PixelShaderConstantCallback(0).Invoke((DependencyObject)sender, e);
#endif
        }

        /// <summary>
        /// Coerces the desaturation factor.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="value">The value.</param>
        /// <returns>New factor.</returns>
        private static object CoerceDesaturationFactor(DependencyObject d, object value)
        {
            var effect = (GrayscaleEffect)d;
            var newFactor = (double)value;

            if (newFactor < 0.0 || newFactor > 1.0)
            {
                return effect.DesaturationFactor;
            }

            return newFactor;
        }
        #endregion
    }
}
