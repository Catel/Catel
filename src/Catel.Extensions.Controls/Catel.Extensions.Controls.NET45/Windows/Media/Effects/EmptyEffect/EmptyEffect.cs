// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyEffect.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Media.Effects
{
    using System;
    using System.Windows.Media.Effects;

    /// <summary>
    /// Empty effect that simply copies the pixel color (so it doesn't make ANY changes to pixels).
    /// </summary>
    /// <remarks>
    /// Implemented to be able to disable pixel shaders in Catel.
    /// <para />
    /// During the build of Catel, this pixel shader effect will not be recompiled to prevent all users to install the
    /// DirectX SDK. If you want to make changes, take a look at the readme of Catel.
    /// </remarks>
    public class EmptyEffect : ShaderEffectBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyEffect"/> class.
        /// </summary>
        public EmptyEffect()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the pixel shader.
        /// </summary>
        /// <returns><see cref="PixelShader"/>.</returns>
        protected override PixelShader CreatePixelShader()
        {
            return new PixelShader() { UriSource = new Uri(@"/Catel.Extensions.Controls;component/Windows/Media/Effects/EmptyEffect/EmptyEffect.ps", UriKind.RelativeOrAbsolute) };
        }
        #endregion
    }
}

#endif