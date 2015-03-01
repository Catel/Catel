// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 3021 // 'type' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute

namespace Catel.MVVM
{
    using System;
    using Catel.MVVM.Views;

#if NETFX_CORE
    using View = global::Windows.UI.Xaml.FrameworkElement;
#elif ANDROID
    using View = global::Android.Views.View;
#elif IOS
    using View = MonoTouch.UIKit.UIView;
#else
    using View = System.Windows.FrameworkElement;
#endif

    /// <summary>
    /// Interface to allow an authentication mechanism to control the CanExecute state of a command.
    /// </summary>
    [CLSCompliant(false)]
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Determines whether the specified <paramref name="command"/> can be executed. The class implementing this interface
        /// can use any required method to check the command.
        /// <para />
        /// It is recommended to use the <see cref="ICatelCommand.Tag"/> property to identify a command.
        /// </summary>
        /// <param name="command">The command that is requested.</param>
        /// <param name="commandParameter">The command parameter.</param>
        /// <returns>
        /// 	<c>true</c> if the specified command can be excecuted; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The <c>CanExecute</c> state of a command is queried a lot. The command itself does not cache any results because
        /// it is not aware of role or identity changes. If caching is required, this must be implemented in the class implementing
        /// the <see cref="IAuthenticationProvider"/> interface.a
        /// </remarks>
        bool CanCommandBeExecuted(ICatelCommand command, object commandParameter);

        /// <summary>
        /// Determines whether the user has access to the specified <paramref name="element"/>. This method is invoked
        /// by the <c>Authentication</c> behavior, and can be used to disable or hide UI elements based on a role or any
        /// other authentication mechanism.
        /// <para />
        /// This method will only be called for UI elements with the <c>Authentication</c> behavior.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="tag">The tag, retrieved from <c>FrameworkElement.Tag</c>.</param>
        /// <param name="authenticationTag">The authentication tag specified by the calling object.</param>
        /// <returns>
        /// 	<c>true</c> if the user has access to the specified UI element; otherwise, <c>false</c>.
        /// </returns>
        [CLSCompliant(false)]
        bool HasAccessToUIElement(View element, object tag, object authenticationTag);
    }
}
