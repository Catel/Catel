// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelWrapperService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Services
{
    using Catel.Logging;
    using Catel.MVVM.Views;
    using Catel.Windows;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Data;
#else
    using Catel.Windows.Controls;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
#endif

    public partial class ViewModelWrapperService
    {
        /// <summary>
        /// Determines whether the specified view is wrapped.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if the view is wrapped; otherwise, <c>false</c>.</returns>
        protected override bool IsViewWrapped(IView view)
        {
            var content = GetContent(view) as FrameworkElement;
            if (content == null || string.Equals(content.Name, InnerWrapperName))
            {
                return true;
            }

            return false;
        }

        private IViewModelWrapper CreateViewModelGrid(IView view, object viewModelSource, WrapOptions wrapOptions)
        {
            var content = GetContent(view) as FrameworkElement;

            var vmGrid = new Grid();
            vmGrid.Name = InnerWrapperName;
            vmGrid.SetBinding(FrameworkElement.DataContextProperty, new Binding { Path = new PropertyPath("ViewModel"), Source = viewModelSource });

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

            Log.Debug("Created target control content wrapper grid for view model");

            return new ViewModelWrapper(vmGrid);
        }
    }
}

#endif