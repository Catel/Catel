// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WrapControlHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if NET || NETCORE

namespace Catel.Windows
{
    using System;
    using System.Windows;
    using Collections;
    using Controls;
    using Reflection;

#if UWP
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Controls.Primitives;
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
#endif

#region Enums
    /// <summary>
    /// Available wrap options that can be used in the <see cref="WrapControlHelper"/>.
    /// </summary>
    [Flags]
    [ObsoleteEx(ReplacementTypeOrMember = "Catel.Service.WrapControlServiceWrapOptions", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
    public enum WrapOptions
    {
        /// <summary>
        /// Generates an inline <see cref="InfoBarMessageControl"/> around the element to wrap.
        /// </summary>
        GenerateInlineInfoBarMessageControl = 1,

        /// <summary>
        /// Generates an overlay <see cref="InfoBarMessageControl"/> around the element to wrap.
        /// </summary>
        GenerateOverlayInfoBarMessageControl = 2,

        /// <summary>
        /// Generates a <see cref="WarningAndErrorValidator"/> for the data context.
        /// </summary>
        GenerateWarningAndErrorValidatorForDataContext = 4,

        /// <summary>
        /// All available options.
        /// </summary>
        All = GenerateInlineInfoBarMessageControl | GenerateWarningAndErrorValidatorForDataContext
    }
#endregion

    /// <summary>
    /// An helper to wrap controls and windows with several controls, such as the <see cref="InfoBarMessageControl"/>.
    /// </summary>
    [ObsoleteEx(ReplacementTypeOrMember = "Catel.Service.WrapControlService", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
    public static class WrapControlHelper
    {
        
#region Constants
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
#endregion
        

        /// <summary>
        /// Determines whether the specified <see cref="FrameworkElement"/> can be safely wrapped.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="FrameworkElement"/> can be safely wrapped; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanBeWrapped(FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(frameworkElement.Name))
            {
                if (frameworkElement.Name.StartsWith(MainContentHolderName))
                {
                    return false;
                }
            }

            return true;
        }

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
        /// framework element can be wrapped by using the <see cref="CanBeWrapped"/> method.
        /// <para />
        /// This method will automatically handle the disconnecting of the framework element from the parent is the <paramref name="parentContentControl"/>
        /// is passed.
        /// </remarks>
        public static Grid Wrap(FrameworkElement frameworkElement, WrapOptions wrapOptions, ContentControl parentContentControl = null)
        {
            return Wrap(frameworkElement, wrapOptions, ArrayShim.Empty<DataWindowButton>(), parentContentControl);
        }

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
        /// framework element can be wrapped by using the <see cref="CanBeWrapped"/> method.
        /// This method will automatically handle the disconnecting of the framework element from the parent is the <paramref name="parentContentControl"/>
        /// is passed.
        /// </remarks>
        public static Grid Wrap(FrameworkElement frameworkElement, WrapOptions wrapOptions, DataWindowButton[] buttons, ContentControl parentContentControl)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNull("buttons", buttons);

            if (!string.IsNullOrWhiteSpace(frameworkElement.Name))
            {
                if (frameworkElement.Name.StartsWith(MainContentHolderName))
                {
                    return (Grid) frameworkElement;
                }
            }

            if (parentContentControl != null)
            {
                SetControlContent(parentContentControl, null);
            }

            var mainContent = frameworkElement;

            // Create the outside grid, so the inner grid is never the same as the main content holder
            var outsideGrid = new Grid();
            outsideGrid.Name = MainContentHolderName.GetUniqueControlName();

            if (Application.Current != null)
            {
                outsideGrid.Resources.MergedDictionaries.Add(Application.Current.Resources);
            }

#region Generate buttons
#if !UWP
            if (buttons.Length > 0)
            {
                // Add wrappanel containing the buttons
                var buttonsWrapPanel = new WrapPanel();
                buttonsWrapPanel.Name = ButtonsWrapPanelName;
#else
                buttonsWrapPanel.SetResourceReference(FrameworkElement.StyleProperty, "DataWindowButtonContainerStyle");
#endif

                foreach (var dataWindowButton in buttons)
                {
                    var button = new Button();
                    if (dataWindowButton.CommandBindingPath != null)
                    {
                        button.SetBinding(ButtonBase.CommandProperty, new Binding(dataWindowButton.CommandBindingPath));
                    }
                    else
                    {
                        button.Command = dataWindowButton.Command;
                    }

                    if (dataWindowButton.ContentBindingPath != null)
                    {
                        Binding contentBinding = new Binding(dataWindowButton.ContentBindingPath);
                        if (dataWindowButton.ContentValueConverter != null)
                        {
                            contentBinding.Converter = dataWindowButton.ContentValueConverter;
                        }
                        button.SetBinding(ButtonBase.ContentProperty, contentBinding);
                    }
                    else
                    {
                        button.Content = dataWindowButton.Text;
                    }

                    if (dataWindowButton.VisibilityBindingPath != null)
                    {
                        Binding visibilityBinding = new Binding(dataWindowButton.VisibilityBindingPath);
                        if (dataWindowButton.VisibilityValueConverter != null)
                        {
                            visibilityBinding.Converter = dataWindowButton.VisibilityValueConverter;
                        }
                        button.SetBinding(ButtonBase.VisibilityProperty, visibilityBinding);
                    }
#if NET || NETCORE
                    button.SetResourceReference(FrameworkElement.StyleProperty, "DataWindowButtonStyle");
                    button.IsDefault = dataWindowButton.IsDefault;
                    button.IsCancel = dataWindowButton.IsCancel;
#else
                    button.Style = Application.Current.Resources["DataWindowButtonStyle"] as Style;
#endif

                    if (dataWindowButton.IsDefault)
                    {
                        button.Name = DefaultOkButtonName;
                    }
                    else if (dataWindowButton.IsCancel)
                    {
                        button.Name = DefaultCancelButtonName;
                    }

                    buttonsWrapPanel.Children.Add(button);
                }

                // Create dockpanel that will dock the buttons underneath the content
                var subDockPanel = new DockPanel();
                subDockPanel.LastChildFill = true;
                DockPanel.SetDock(buttonsWrapPanel, Dock.Bottom);
                subDockPanel.Children.Add(buttonsWrapPanel);

                // Add actual content
                subDockPanel.Children.Add(frameworkElement);

                // The dockpanel is now the main content
                mainContent = subDockPanel;
            }
#endif
#endregion

#region Generate internal grid
            // Create grid
            var internalGrid = new Grid();
            internalGrid.Name = InternalGridName;
            internalGrid.Children.Add(mainContent);

            // Grid is now the main content
            mainContent = internalGrid;
#endregion

#region Generate WarningAndErrorValidator
            if (Enum<WrapOptions>.Flags.IsFlagSet(wrapOptions, WrapOptions.GenerateWarningAndErrorValidatorForDataContext))
            {
                // Create warning and error validator
                var warningAndErrorValidator = new WarningAndErrorValidator();
                warningAndErrorValidator.Name = WarningAndErrorValidatorName;
                warningAndErrorValidator.SetBinding(WarningAndErrorValidator.SourceProperty, new Binding());

                // Add to grid
                internalGrid.Children.Add(warningAndErrorValidator);
            }
#endregion

#region Generate InfoBarMessageControl
#if !UWP
            if (Enum<WrapOptions>.Flags.IsFlagSet(wrapOptions, WrapOptions.GenerateInlineInfoBarMessageControl) ||
                Enum<WrapOptions>.Flags.IsFlagSet(wrapOptions, WrapOptions.GenerateOverlayInfoBarMessageControl))
            {
                // Create info bar message control
                var infoBarMessageControl = new InfoBarMessageControl();
                infoBarMessageControl.Name = InfoBarMessageControlName;
                infoBarMessageControl.Content = mainContent;

                if (Enum<WrapOptions>.Flags.IsFlagSet(wrapOptions, WrapOptions.GenerateOverlayInfoBarMessageControl))
                {
                    infoBarMessageControl.Mode = InfoBarMessageControlMode.Overlay;
                }

                // This is now the main content
                mainContent = infoBarMessageControl;
            }
#endif
#endregion

            // Set content of the outside grid
            outsideGrid.Children.Add(mainContent);

            if (parentContentControl != null)
            {
                SetControlContent(parentContentControl, outsideGrid);
            }

            return outsideGrid;
        }

        /// <summary>
        /// Gets a wrapped element mapped by the <paramref name="wrapOption"/>.
        /// </summary>
        /// <typeparam name="T">Type of the control to return.</typeparam>
        /// <param name="wrappedGrid">The wrapped grid.</param>
        /// <param name="wrapOption">The wrap option that is used, which will be mapped to the control. The value <see cref="WrapOptions.All"/> is not allowed and will throw an exception.</param>
        /// <returns>
        /// 	<see cref="FrameworkElement"/> or <c>null</c> if the element is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="wrappedGrid"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="wrapOption"/> is <see cref="WrapOptions.All"/>.</exception>
        public static T GetWrappedElement<T>(Grid wrappedGrid, WrapOptions wrapOption)
            where T : FrameworkElement
        {
            return GetWrappedElement(wrappedGrid, wrapOption) as T;
        }

        /// <summary>
        /// Gets a wrapped element mapped by the <paramref name="wrapOption"/>.
        /// </summary>
        /// <param name="wrappedGrid">The wrapped grid.</param>
        /// <param name="wrapOption">The wrap option that is used, which will be mapped to the control. The value <see cref="WrapOptions.All"/> is not allowed and will throw an exception.</param>
        /// <returns>
        /// 	<see cref="FrameworkElement"/> or <c>null</c> if the element is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="wrappedGrid"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="wrapOption"/> is <see cref="WrapOptions.All"/>.</exception>
        public static FrameworkElement GetWrappedElement(Grid wrappedGrid, WrapOptions wrapOption)
        {
            Argument.IsNotNull("wrappedGrid", wrappedGrid);

            if (wrapOption == WrapOptions.All)
            {
                throw new ArgumentOutOfRangeException("wrapOption");
            }

            switch (wrapOption)
            {
                case WrapOptions.GenerateInlineInfoBarMessageControl:
                    return GetWrappedElement(wrappedGrid, InfoBarMessageControlName);

                case WrapOptions.GenerateWarningAndErrorValidatorForDataContext:
                    return GetWrappedElement(wrappedGrid, WarningAndErrorValidatorName);
            }

            return null;
        }

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
        public static T GetWrappedElement<T>(Grid wrappedGrid, string controlName)
            where T : FrameworkElement
        {
            return GetWrappedElement(wrappedGrid, controlName) as T;
        }

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
        public static FrameworkElement GetWrappedElement(Grid wrappedGrid, string controlName)
        {
            Argument.IsNotNull("wrappedGrid", wrappedGrid);
            Argument.IsNotNullOrEmpty("controlName", controlName);

            if ((controlName != DefaultOkButtonName) &&
                (controlName != DefaultCancelButtonName) &&
                (controlName != InfoBarMessageControlName) &&
                (controlName != WarningAndErrorValidatorName))
            {
                throw new ArgumentOutOfRangeException("controlName");
            }

            return wrappedGrid.FindVisualDescendantByName(controlName) as FrameworkElement;
        }

        /// <summary>
        /// Sets the content of the control via reflection.
        /// </summary>
        /// <param name="contentControl">The content control.</param>
        /// <param name="element">The element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="contentControl"/> is <c>null</c>.</exception>
        private static void SetControlContent(object contentControl, FrameworkElement element)
        {
            Argument.IsNotNull("contentControl", contentControl);

            var propertyInfo = contentControl.GetType().GetPropertyEx("Content");
            propertyInfo.SetValue(contentControl, element, null);
        }
    }
}

#endif
