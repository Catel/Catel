namespace Catel.Services
{
    using Catel.Logging;
    using Catel.MVVM.Views;
    using Catel.Reflection;
    using Catel.Windows.Controls;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Page = System.Windows.Controls.Page;
    using UserControl = System.Windows.Controls.UserControl;

    public partial class ViewModelWrapperService
    {
        private const string InnerWrapperName = "__catelInnerWrapper";

        /// <summary>
        /// Determines whether the specified view is wrapped.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if the view is wrapped; otherwise, <c>false</c>.</returns>
        protected override bool IsViewWrapped(IView view)
        {
            var content = GetContent(view) as FrameworkElement;
            if (content is null)
            {
                return true;
            }

            if (content.Name.StartsWith(InnerWrapperName))
            {
                var binding = content.GetBindingExpression(FrameworkElement.DataContextProperty);
                if (binding is not null)
                {
                    return true;
                }
            }

            return false;
        }

        private IViewModelWrapper CreateViewModelGrid(IView view, object viewModelSource, WrapOptions wrapOptions)
        {
            var content = GetContent(view) as FrameworkElement;
            if (!Enum<WrapOptions>.Flags.IsFlagSet(wrapOptions, WrapOptions.Force) && content is null)
            {
                return null;
            }

            var viewTypeName = view.GetType().Name;

            Grid vmGrid = null;

            var existingGrid = GetContent(view) as Grid;
            if (existingGrid is not null)
            {
                if (existingGrid.Name.StartsWith(InnerWrapperName))
                {
                    Log.Debug($"No need to create content wrapper grid for view model for view '{viewTypeName}', custom grid with special name defined");

                    vmGrid = existingGrid;
                }
            }

            if (vmGrid is null)
            {
                Log.Debug($"Creating content wrapper grid for view model for view '{viewTypeName}'");

                vmGrid = new Grid();
                vmGrid.Name = InnerWrapperName.GetUniqueControlName();

                if (Enum<WrapOptions>.Flags.IsFlagSet(wrapOptions, WrapOptions.CreateWarningAndErrorValidatorForViewModel))
                {
                    var warningAndErrorValidator = new WarningAndErrorValidator();
                    warningAndErrorValidator.SetBinding(WarningAndErrorValidator.SourceProperty, new Binding());

                    vmGrid.Children.Add(warningAndErrorValidator);
                }

                SetContent(view, null);

                if (content is not null)
                {
                    vmGrid.Children.Add(content);
                }

                SetContent(view, vmGrid);

                Log.Debug($"Created content wrapper grid for view model for view '{viewTypeName}'");
            }

            var binding = vmGrid.GetBindingExpression(FrameworkElement.DataContextProperty);
            if (binding is null)
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
            var userControl = view as UserControl;
            if (userControl is not null)
            {
                var content = userControl.Content as FrameworkElement;
                return content;
            }

            var contentControl = view as ContentControl;
            if (contentControl is not null)
            {
                var content = contentControl.Content as FrameworkElement;
                return content;
            }

            var page = view as Page;
            if (page is not null)
            {
                var content = page.Content as FrameworkElement;
                return content;
            }

            var lastResortContent = PropertyHelper.GetPropertyValue(view, "Content", false);
            return lastResortContent;
        }

        private void SetContent(IView view, object content)
        {
            var userControl = view as UserControl;
            if (userControl is not null)
            {
                userControl.Content = (UIElement)content;
                return;
            }

            var contentControl = view as ContentControl;
            if (contentControl is not null)
            {
                contentControl.Content = content;
                return;
            }

            var page = view as Page;
            if (page is not null)
            {
                // Note: cast required or SL
                page.Content = (UIElement)content;
                return;
            }

            PropertyHelper.SetPropertyValue(view, "Content", content, false);
        }
    }
}
