namespace Catel.MVVM.Views;

using System;
using System.Windows;

public interface IViewFactory
{
    FrameworkElement? CreateView(Type viewType);
    FrameworkElement? CreateViewWithViewModel(Type viewType, object? dataContext);
}
