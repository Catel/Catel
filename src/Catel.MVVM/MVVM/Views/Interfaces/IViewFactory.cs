namespace Catel.MVVM.Views
{
    using System;
    using System.Windows;

    public interface IViewFactory
    {
        FrameworkElement? ConstructViewWithViewModel(Type viewType, object? dataContext);
    }
}