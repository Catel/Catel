// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShaderEffectBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Media.Effects
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Effects;

    using Logging;

    /// <summary>
    /// Base class for shader effects in Catel.
    /// </summary>
    public abstract class ShaderEffectBase : ShaderEffect
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static PixelShader _pixelShader;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ShaderEffectBase"/> class.
        /// </summary>
        static ShaderEffectBase()
        {
#if !SILVERLIGHT
            // Subscribe to events so the software doesn't crash on invalid pixel shaders
            PixelShader.InvalidPixelShaderEncountered += OnPixelShaderInvalidPixelShaderEncountered;
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Media.Effects.ShaderEffect"/> class.
        /// </summary>
        protected ShaderEffectBase()
        {
            try
            {
                InitializePixelShader();

                UpdateShaderValue(InputProperty);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize shader");
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this shader effect is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this shader effect is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get { return StyleHelper.PixelShaderMode != PixelShaderMode.Off; } }

        /// <summary>
        /// Gets or sets the input brush.
        /// </summary>
        /// <value>The input.</value>
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        /// <summary>
        /// Property definition for <see cref="Input"/>.
        /// </summary>
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(ShaderEffectBase), 0);
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the pixel shader.
        /// </summary>
        private void InitializePixelShader()
        {
            if (_pixelShader == null)
            {
                if (!IsEnabled)
                {
                    Log.Debug("Pixel shaders are disabled, using default one that is safe");

                    _pixelShader = new PixelShader() { UriSource = new Uri(@"/Catel.Extensions.Controls;component/Windows/Media/Effects/EmptyEffect/EmptyEffect.ps", UriKind.RelativeOrAbsolute) };

#if !SILVERLIGHT
                    // Only render in software (so limited video cards can break the software)
                    _pixelShader.ShaderRenderMode = ShaderRenderMode.SoftwareOnly;
#endif
                }
                else
                {
                    _pixelShader = CreatePixelShader();

#if !SILVERLIGHT
                    switch (StyleHelper.PixelShaderMode)
                    {
                        case PixelShaderMode.Auto:
                            _pixelShader.ShaderRenderMode = ShaderRenderMode.Auto;
                            break;

                        case PixelShaderMode.Hardware:
                            Log.Debug("Forcing hardware rendering for pixel shader '{0}'", GetType());

                            _pixelShader.ShaderRenderMode = ShaderRenderMode.HardwareOnly;
                            break;

                        case PixelShaderMode.Software:
                            Log.Debug("Forcing software rendering for pixel shader '{0}'", GetType());

                            _pixelShader.ShaderRenderMode = ShaderRenderMode.SoftwareOnly;
                            break;
                    }
#endif
                }
            }

            PixelShader = _pixelShader;
        }

        /// <summary>
        /// Creates the pixel shader.
        /// </summary>
        /// <returns><see cref="PixelShader"/>.</returns>
        protected abstract PixelShader CreatePixelShader();

#if !SILVERLIGHT
        /// <summary>
        /// Handles the InvalidPixelShaderEncountered event of the PixelShader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This is added to prevent a real crash on the app because of an invalid pixel shader.
        /// </remarks>
        private static void OnPixelShaderInvalidPixelShaderEncountered(object sender, EventArgs e)
        {
            Log.Warning("Invalid PixelShader Encountered. Occurs when the render thread cannot process the pixel shader.");
        }
#endif
        #endregion
    }
}
