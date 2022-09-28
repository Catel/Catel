namespace Catel.Services
{
    using System;
    using System.Windows;
    using Windows;

    using Catel.Windows.Controls;
    using Collections;
    using Reflection;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;

    /// <summary>
    /// An helper to wrap controls and windows with several controls, such as the <see cref="InfoBarMessageControl"/>.
    /// </summary>
    public class WrapControlService : IWrapControlService
    {
        /// <summary>
        /// Determines whether the specified <see cref="FrameworkElement"/> can be safely wrapped.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="FrameworkElement"/> can be safely wrapped; otherwise, <c>false</c>.
        /// </returns>
        public bool CanBeWrapped(FrameworkElement frameworkElement)
        {
            if (frameworkElement is null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(frameworkElement.Name))
            {
                if (frameworkElement.Name.StartsWith(WrapControlServiceControlNames.MainContentHolderName))
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
        public Grid Wrap(FrameworkElement frameworkElement, WrapControlServiceWrapOptions wrapOptions, ContentControl parentContentControl = null)
        {
            return Wrap(frameworkElement, wrapOptions, Array.Empty<DataWindowButton>(), parentContentControl);
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
        public Grid Wrap(FrameworkElement frameworkElement, WrapControlServiceWrapOptions wrapOptions, DataWindowButton[] buttons, ContentControl parentContentControl)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNull("buttons", buttons);

            if (!string.IsNullOrWhiteSpace(frameworkElement.Name))
            {
                if (frameworkElement.Name.StartsWith(WrapControlServiceControlNames.MainContentHolderName))
                {
                    return (Grid)frameworkElement;
                }
            }

            if (parentContentControl is not null)
            {
                SetControlContent(parentContentControl, null);
            }

            var mainContent = frameworkElement;

            // Create the outside grid, so the inner grid is never the same as the main content holder
            var outsideGrid = new Grid
            {
                Name = WrapControlServiceControlNames.MainContentHolderName.GetUniqueControlName()
            };

            if (Enum<WrapControlServiceWrapOptions>.Flags.IsFlagSet(wrapOptions, WrapControlServiceWrapOptions.ExplicitlyAddApplicationResourcesDictionary))
            {
                if (Application.Current is not null)
                {
                    outsideGrid.Resources.MergedDictionaries.Add(Application.Current.Resources);
                }
            }

            #region Generate buttons
            if (buttons.Length > 0)
            {
                // Add wrappanel containing the buttons
                var buttonsWrapPanel = new WrapPanel
                {
                    Name = WrapControlServiceControlNames.ButtonsWrapPanelName
                };

                buttonsWrapPanel.SetResourceReference(FrameworkElement.StyleProperty, "DataWindowButtonContainerStyle");

                foreach (var dataWindowButton in buttons)
                {
                    var button = new Button();
                    if (dataWindowButton.CommandBindingPath is not null)
                    {
                        button.SetBinding(ButtonBase.CommandProperty, new Binding(dataWindowButton.CommandBindingPath));
                    }
                    else
                    {
                        button.Command = dataWindowButton.Command;
                    }

                    if (dataWindowButton.ContentBindingPath is not null)
                    {
                        var contentBinding = new Binding(dataWindowButton.ContentBindingPath);
                        if (dataWindowButton.ContentValueConverter is not null)
                        {
                            contentBinding.Converter = dataWindowButton.ContentValueConverter;
                        }
                        button.SetBinding(ButtonBase.ContentProperty, contentBinding);
                    }
                    else
                    {
                        button.Content = dataWindowButton.Text;
                    }

                    if (dataWindowButton.VisibilityBindingPath is not null)
                    {
                        var visibilityBinding = new Binding(dataWindowButton.VisibilityBindingPath);
                        if (dataWindowButton.VisibilityValueConverter is not null)
                        {
                            visibilityBinding.Converter = dataWindowButton.VisibilityValueConverter;
                        }
                        button.SetBinding(ButtonBase.VisibilityProperty, visibilityBinding);
                    }

                    button.SetResourceReference(FrameworkElement.StyleProperty, "DataWindowButtonStyle");
                    button.IsDefault = dataWindowButton.IsDefault;
                    button.IsCancel = dataWindowButton.IsCancel;

                    if (dataWindowButton.IsDefault)
                    {
                        button.Name = WrapControlServiceControlNames.DefaultOkButtonName;
                    }
                    else if (dataWindowButton.IsCancel)
                    {
                        button.Name = WrapControlServiceControlNames.DefaultCancelButtonName;
                    }

                    buttonsWrapPanel.Children.Add(button);
                }

                // Create dockpanel that will dock the buttons underneath the content
                var subDockPanel = new DockPanel
                {
                    LastChildFill = true
                };
                DockPanel.SetDock(buttonsWrapPanel, Dock.Bottom);
                subDockPanel.Children.Add(buttonsWrapPanel);

                // Add actual content
                subDockPanel.Children.Add(frameworkElement);

                // The dockpanel is now the main content
                mainContent = subDockPanel;
            }
            #endregion

            #region Generate internal grid
            // Create grid
            var internalGrid = new Grid
            {
                Name = WrapControlServiceControlNames.InternalGridName
            };

            if (Enum<WrapControlServiceWrapOptions>.Flags.IsFlagSet(wrapOptions, WrapControlServiceWrapOptions.GenerateAdornerDecorator))
            {
                var adornerDecorator = new AdornerDecorator
                {
                    Child = mainContent
                };

                mainContent = adornerDecorator;
            }

            internalGrid.Children.Add(mainContent);

            // Grid is now the main content
            mainContent = internalGrid;
            #endregion

            #region Generate WarningAndErrorValidator
            if (Enum<WrapControlServiceWrapOptions>.Flags.IsFlagSet(wrapOptions, WrapControlServiceWrapOptions.GenerateWarningAndErrorValidatorForDataContext))
            {
                // Create warning and error validator
                var warningAndErrorValidator = new WarningAndErrorValidator
                {
                    Name = WrapControlServiceControlNames.WarningAndErrorValidatorName
                };
                warningAndErrorValidator.SetBinding(WarningAndErrorValidator.SourceProperty, new Binding());

                // Add to grid
                internalGrid.Children.Add(warningAndErrorValidator);
            }
            #endregion

            #region Generate InfoBarMessageControl
            if (Enum<WrapControlServiceWrapOptions>.Flags.IsFlagSet(wrapOptions, WrapControlServiceWrapOptions.GenerateInlineInfoBarMessageControl) ||
                Enum<WrapControlServiceWrapOptions>.Flags.IsFlagSet(wrapOptions, WrapControlServiceWrapOptions.GenerateOverlayInfoBarMessageControl))
            {
                // Create info bar message control
                var infoBarMessageControl = new InfoBarMessageControl
                {
                    Name = WrapControlServiceControlNames.InfoBarMessageControlName,
                    Content = mainContent
                };

                if (Enum<WrapControlServiceWrapOptions>.Flags.IsFlagSet(wrapOptions, WrapControlServiceWrapOptions.GenerateOverlayInfoBarMessageControl))
                {
                    infoBarMessageControl.Mode = InfoBarMessageControlMode.Overlay;
                }

                // This is now the main content
                mainContent = infoBarMessageControl;
            }
            #endregion

            // Set content of the outside grid
            outsideGrid.Children.Add(mainContent);

            if (parentContentControl is not null)
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
        /// <param name="wrapOption">The wrap option that is used, which will be mapped to the control. The value <see cref="WrapControlServiceWrapOptions.All"/> is not allowed and will throw an exception.</param>
        /// <returns>
        /// 	<see cref="FrameworkElement"/> or <c>null</c> if the element is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="wrappedGrid"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="wrapOption"/> is <see cref="WrapControlServiceWrapOptions.All"/>.</exception>
        public T GetWrappedElement<T>(Grid wrappedGrid, WrapControlServiceWrapOptions wrapOption)
            where T : FrameworkElement
        {
            return GetWrappedElement(wrappedGrid, wrapOption) as T;
        }

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
        public FrameworkElement GetWrappedElement(Grid wrappedGrid, WrapControlServiceWrapOptions wrapOption)
        {
            Argument.IsNotNull("wrappedGrid", wrappedGrid);

            if (wrapOption == WrapControlServiceWrapOptions.All)
            {
                throw new ArgumentOutOfRangeException("wrapOption");
            }

            switch (wrapOption)
            {
                case WrapControlServiceWrapOptions.GenerateInlineInfoBarMessageControl:
                    return GetWrappedElement(wrappedGrid, WrapControlServiceControlNames.InfoBarMessageControlName);

                case WrapControlServiceWrapOptions.GenerateWarningAndErrorValidatorForDataContext:
                    return GetWrappedElement(wrappedGrid, WrapControlServiceControlNames.WarningAndErrorValidatorName);
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
        public T GetWrappedElement<T>(Grid wrappedGrid, string controlName)
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
        public FrameworkElement GetWrappedElement(Grid wrappedGrid, string controlName)
        {
            Argument.IsNotNull("wrappedGrid", wrappedGrid);
            Argument.IsNotNullOrEmpty("controlName", controlName);

            if ((controlName != WrapControlServiceControlNames.DefaultOkButtonName) &&
                (controlName != WrapControlServiceControlNames.DefaultCancelButtonName) &&
                (controlName != WrapControlServiceControlNames.InfoBarMessageControlName) &&
                (controlName != WrapControlServiceControlNames.WarningAndErrorValidatorName))
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
