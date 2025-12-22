namespace Catel.MVVM.Views
{
    using System;
    using System.Windows;

    public interface IViewFactory
    {
        FrameworkElement? CreateViewWithViewModel(Type viewType, object? dataContext);
    }
}