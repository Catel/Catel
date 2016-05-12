// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWrapperService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Services
{
    using System;
    using System.Runtime.CompilerServices;
    using Catel.Logging;
    using Catel.MVVM.Views;
    using Catel.Reflection;
    using Catel.Windows;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Data;
    using Page = global::Windows.UI.Xaml.Controls.Page;
#else
    using Catel.Windows.Controls;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Page = System.Windows.Controls.Page;
#endif

    public partial class ViewModelWrapperService
    {
        private const string InnerWrapperName = "__catelInnerWrapper";

        private readonly ConditionalWeakTable<object, object> _weakIsWrappingTable = new ConditionalWeakTable<object, object>();

        /// <summary>
        /// Determines whether the specified view is wrapped.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if the view is wrapped; otherwise, <c>false</c>.</returns>
        protected override bool IsViewWrapped(IView view)
        {
            var content = GetContent(view) as FrameworkElement;
            if (content == null)
            {
                return true;
            }

            if (content.Name.StartsWith(InnerWrapperName))
            {
                var binding = content.GetBindingExpression(FrameworkElement.DataContextProperty);
                if (binding != null)
                {
                    return true;
                }
            }

            return false;
        }

        private IViewModelWrapper CreateViewModelGrid(IView view, object viewModelSource, WrapOptions wrapOptions)
        {
            var content = GetContent(view) as FrameworkElement;
            if (content == null)
            {
                return null;
            }

            object tempObj = null;
            if (_weakIsWrappingTable.TryGetValue(view, out tempObj))
            {
                return null;
            }

            var viewTypeName = view.GetType().Name;

            _weakIsWrappingTable.Add(view, new object());

            Grid vmGrid = null;

            var existingGrid = GetContent(view) as Grid;
            if (existingGrid != null)
            {
                if (existingGrid.Name.StartsWith(InnerWrapperName))
                {
                    Log.Debug($"No need to create content wrapper grid for view model for view '{viewTypeName}', custom grid with special name defined");

                    vmGrid = existingGrid;
                }
            }

            if (vmGrid == null)
            {
                Log.Debug($"Creating content wrapper grid for view model for view '{viewTypeName}'");

                vmGrid = new Grid();
                vmGrid.Name = InnerWrapperName.GetUniqueControlName();

#if NET || SL5
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

                Log.Debug($"Created content wrapper grid for view model for view '{viewTypeName}'");
            }

            var binding = vmGrid.GetBindingExpression(FrameworkElement.DataContextProperty);
            if (binding == null)
            {
                vmGrid.SetBinding(FrameworkElement.DataContextProperty, new Binding
                {
                    Path = new PropertyPath("ViewModel"),
                    Source = viewModelSource
                });
            }

            return new ViewModelWrapper(vmGrid);
        }

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
                    content = PropertyHelper.GetPropertyValue(view, "Content", false);
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

            PropertyHelper.SetPropertyValue(view, "Content", content, false);
        }
    }
}

#endif