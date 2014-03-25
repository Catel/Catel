// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWrapperService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using Catel.Logging;
    using Catel.MVVM.Views;
    using Catel.Reflection;
    
#if XAMARIN

#elif NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Data;
    using Page = global::Windows.UI.Xaml.Controls.Page;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Page = System.Windows.Controls.Page;
#endif



#if !XAMARIN
    using Catel.Windows;
#endif

    /// <summary>
    /// The view model wrapper service which is responsible of ensuring the view model container layer.
    /// </summary>
    public class ViewModelWrapperService : IViewModelWrapperService
    {
        private const string InnerWrapperName = "__catelInnerWrapper";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Determines whether the specified view is already wrapped.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if the specified view is already wrapped; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public bool IsWrapped(IView view)
        {
            Argument.IsNotNull("view", view);

#if XAMARIN
            throw new MustBeImplementedException();
#else
            var content = GetContent(view) as FrameworkElement;
            if (content == null || string.Equals(content.Name, InnerWrapperName))
            {
                return true;
            }

            return false;
#endif
        }

        /// <summary>
        /// Wraps the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModelSource">The view model source containing the <c>ViewModel</c> property.</param>
        /// <param name="wrapOptions">The wrap options.</param>
        /// <returns>The <see cref="IViewModelWrapper" />.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
        public IViewModelWrapper Wrap(IView view, object viewModelSource, WrapOptions wrapOptions)
        {
            Argument.IsNotNull("view", view);
            Argument.IsNotNull("viewModelSource", viewModelSource);

#if XAMARIN
            throw new MustBeImplementedException();
#else
            return CreateViewModelGrid(view, viewModelSource, wrapOptions);
#endif
        }

#if !XAMARIN
        private IViewModelWrapper CreateViewModelGrid(IView view, object viewModelSource, WrapOptions wrapOptions)
        {
            var content = GetContent(view) as FrameworkElement;

            var vmGrid = new Grid();
            vmGrid.Name = InnerWrapperName;
            vmGrid.SetBinding(FrameworkElement.DataContextProperty, new Binding { Path = new PropertyPath("ViewModel"), Source = viewModelSource });

#if NET || SL4 || SL5
            if (Enum<WrapOptions>.Flags.IsFlagSet(wrapOptions, WrapOptions.CreateWarningAndErrorValidatorForViewModel))
            {
                var warningAndErrorValidator = new WarningAndErrorValidator();
                warningAndErrorValidator.SetBinding(WarningAndErrorValidator.SourceProperty, new Binding());

                vmGrid.Children.Add(warningAndErrorValidator);
            }
#endif

            if (Enum<WrapOptions>.Flags.IsFlagSet(wrapOptions, WrapOptions.TransferStylesAndTransitionsToViewModelGrid))
            {
                content.TransferStylesAndTransitions(vmGrid);
            }

            SetContent(view, null);
            vmGrid.Children.Add(content);
            SetContent(view, vmGrid);

            Log.Debug("Created target control content wrapper grid for view model");

            return new ViewModelWrapper(vmGrid);
        }
#endif

        private object GetContent(IView view)
        {
            object content = null;

            var contentControl = view as ContentControl;
            if (contentControl != null)
            {
                content = contentControl.Content as FrameworkElement;
            }
            else
            {
                var page = view as Page;
                if (page != null)
                {
                    content = page.Content as FrameworkElement;
                }
                else
                {
                    content = PropertyHelper.GetPropertyValue(view, "Content");
                }
            }

            return content;
        }

        private void SetContent(IView view, object content)
        {
            var contentControl = view as ContentControl;
            if (contentControl != null)
            {
                contentControl.Content = content;
                return;
            }

            var page = view as Page;
            if (page != null)
            {
                // Note: cast required or SL
                page.Content = (UIElement)content;
                return;
            }

            PropertyHelper.SetPropertyValue(view, "Content", content);
        }
    }
}