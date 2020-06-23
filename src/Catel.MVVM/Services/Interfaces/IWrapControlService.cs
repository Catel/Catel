// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWrapControlService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Services
{
    using System;

#if UWP
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
#else
    using System.Windows;
    using System.Windows.Controls;
#endif

    using Catel.Windows;
    using Catel.Windows.Controls;

    /// <summary>
    /// Available wrap options that can be used in the <see cref="IWrapControlService"/>.
    /// </summary>
    [Flags]
    public enum WrapControlServiceWrapOptions
    {
        /// <summary>
        /// Generates an inline InfoBarMessageControl around the element to wrap.
        /// </summary>
        GenerateInlineInfoBarMessageControl = 1,

        /// <summary>
        /// Generates an overlay InfoBarMessageControl around the element to wrap.
        /// </summary>
        GenerateOverlayInfoBarMessageControl = 2,

        /// <summary>
        /// Generates a <see cref="WarningAndErrorValidator"/> for the data context.
        /// </summary>
        GenerateWarningAndErrorValidatorForDataContext = 4,

        /// <summary>
        /// Explicitly add the application resources to the resource dictionary.
        /// </summary>
        ExplicitlyAddApplicationResourcesDictionary = 8,

        /// <summary>
        /// Add adorner decorator.
        /// </summary>
        GenerateAdornerDecorator = 16,

        /// <summary>
        /// All available options.
        /// </summary>
        All = GenerateInlineInfoBarMessageControl | GenerateWarningAndErrorValidatorForDataContext | ExplicitlyAddApplicationResourcesDictionary | GenerateAdornerDecorator
    }

    /// <summary>
    /// The wrap control service control names.
    /// </summary>
    public static class WrapControlServiceControlNames
    {
        /// <summary>
        /// The name of the internal grid. Retrieve the grid with this name to add custom controls to the inner grid.
        /// </summary>
        public const string InternalGridName = "_InternalGridName";

        /// <summary>
        /// The name of the wrap panel that contains the buttons.
        /// </summary>
        public const string ButtonsWrapPanelName = "_ButtonsWrapPanel";

        /// <summary>
        /// The name of the main content holder, used to prevent that an element is wrapped multiple times.
        /// </summary>
        public const string MainContentHolderName = "_MainContentHolder";

        /// <summary>
        /// The name of the info bar message control.
        /// </summary>
        public const string InfoBarMessageControlName = "_InfoBarMessageControl";

        /// <summary>
        /// The name of the warning and error validator control.
        /// </summary>
        public const string WarningAndErrorValidatorName = "_WarningAndErrorValidator";

        /// <summary>
        /// The name of the default ok button.
        /// </summary>
        public const string DefaultOkButtonName = "okButton";

        /// <summary>
        /// The name of the default cancel button.
        /// </summary>
        public const string DefaultCancelButtonName = "cancelButton";
    }

    /// <summary>
    /// The wrap control service interface.
    /// </summary>
    public interface IWrapControlService
    {
        /// <summary>
        /// Determines whether the specified <see cref="FrameworkElement"/> can be safely wrapped.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="FrameworkElement"/> can be safely wrapped; otherwise, <c>false</c>.
        /// </returns>
        bool CanBeWrapped(FrameworkElement frameworkElement);

        /// <summary>
        /// Wraps the specified framework element without any buttons.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="wrapOptions">The wrap options.</param>
        /// <param name="parentContentControl">The parent content control.</param>
        /// <returns>
        /// 	<see cref="Grid"/> that contains the wrapped content.
        /// </returns>
        /// <remarks>
        /// The framework element that is passed must be disconnected from the parent first. It is recommended to first check whether a
        /// framework element can be wrapped by using the <see cref="IWrapControlService.CanBeWrapped"/> method.
        /// <para />
        /// This method will automatically handle the disconnecting of the framework element from the parent is the <paramref name="parentContentControl"/>
        /// is passed.
        /// </remarks>
        Grid Wrap(FrameworkElement frameworkElement, WrapControlServiceWrapOptions wrapOptions, ContentControl parentContentControl = null);

        /// <summary>
        /// Wraps the specified framework element.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="wrapOptions">The wrap options.</param>
        /// <param name="buttons">The buttons to add.</param>
        /// <param name="parentContentControl">The parent content control.</param>
        /// <returns><see cref="Grid"/> that contains the wrapped content.</returns>
        /// <remarks>
        /// The framework element that is passed must be disconnected from the parent first. It is recommended to first check whether a
        /// framework element can be wrapped by using the <see cref="IWrapControlService.CanBeWrapped"/> method.
        /// This method will automatically handle the disconnecting of the framework element from the parent is the <paramref name="parentContentControl"/>
        /// is passed.
        /// </remarks>
        Grid Wrap(FrameworkElement frameworkElement, WrapControlServiceWrapOptions wrapOptions, DataWindowButton[] buttons, ContentControl parentContentControl);

        /// <summary>
        /// Gets a wrapped element mapped by the <paramref name="wrapOption"/>.
        /// </summary>
        /// <typeparam name="T">Type of the control to return.</typeparam>
        /// <param name="wrappedGrid">The wrapped grid.</param>
        /// <param name="wrapOption">The wrap option that is used, which will be mapped to the control. The value <see cref="WrapControlServiceWrapOptions.All"/> is not allowed and will throw an exception.</param>
        /// <returns>
        /// 	<see cref="FrameworkElement"/> or <c>null</c> if the element is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="wrappedGrid"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="wrapOption"/> is <see cref="WrapControlServiceWrapOptions.All"/>.</exception>
        T GetWrappedElement<T>(Grid wrappedGrid, WrapControlServiceWrapOptions wrapOption)
            where T : FrameworkElement;

        /// <summary>
        /// Gets a wrapped element mapped by the <paramref name="wrapOption"/>.
        /// </summary>
        /// <param name="wrappedGrid">The wrapped grid.</param>
        /// <param name="wrapOption">The wrap option that is used, which will be mapped to the control. The value <see cref="WrapControlServiceWrapOptions.All"/> is not allowed and will throw an exception.</param>
        /// <returns>
        /// 	<see cref="FrameworkElement"/> or <c>null</c> if the element is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="wrappedGrid"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="wrapOption"/> is <see cref="WrapControlServiceWrapOptions.All"/>.</exception>
        FrameworkElement GetWrappedElement(Grid wrappedGrid, WrapControlServiceWrapOptions wrapOption);

        /// <summary>
        /// Gets a wrapped element by name.
        /// </summary>
        /// <typeparam name="T">Type of the control to return.</typeparam>
        /// <param name="wrappedGrid">The wrapped grid.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>
        /// 	<see cref="FrameworkElement"/> or <c>null</c> if the element is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="wrappedGrid"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="controlName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="controlName"/> is not a valid control name.</exception>
        T GetWrappedElement<T>(Grid wrappedGrid, string controlName)
            where T : FrameworkElement;

        /// <summary>
        /// Gets a wrapped element by name.
        /// </summary>
        /// <param name="wrappedGrid">The wrapped grid.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>
        /// 	<see cref="FrameworkElement"/> or <c>null</c> if the element is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="wrappedGrid"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="controlName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="controlName"/> is not a valid control name.</exception>
        FrameworkElement GetWrappedElement(Grid wrappedGrid, string controlName);
    }
}

#endif
