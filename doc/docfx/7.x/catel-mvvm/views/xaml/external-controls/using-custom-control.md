---
title: "Using a custom control" 
---
In this part of the documentation, the `RadTabItem` of Telerik will be used as an example on how to create a `RadTabItem` that behaves like the `UserControl`.

## Creating the base class with behavior

The first thing to do is to create a new base class that accepts a view model type argument. In this example, we will call it `TabItem` (to make it as "external control company independent" as possible). Below is the code for the control definition. The downside of xaml based applications is that you cannot derive from controls or windows that have a partial class defined in xaml. Therefore, all controls and code must be initialized via code as shown, in the code below.

```
/// <summary>
/// Base class for a control with the Catel mvvm behavior.
/// </summary>
public class TabItem : RadTabItem, IUserControl
{
   private readonly UserControlLogic _logic;

   private event EventHandler<EventArgs> _viewLoaded;
   private event EventHandler<EventArgs> _viewUnloaded;
   private event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs> _viewDataContextChanged;

   /// <summary>
   /// Initializes a new instance of the <see cref="TabItem"/> class.
   /// </summary>
   public TabItem()
   {
      _logic = new UserControlLogic(this, viewModelType);
      _logic.PropertyChanged += (sender, e) => PropertyChanged.SafeInvoke(this, e);

      Loaded += (sender, e) => _viewLoaded.SafeInvoke(this);
      Unloaded += (sender, e) => _viewUnloaded.SafeInvoke(this);
      this.AddDataContextChangedHandler((sender, e) => _viewDataContextChanged.SafeInvoke(this, new Catel.MVVM.Views.DataContextChangedEventArgs(e.OldValue, e.NewValue)));

      SetBinding(RadTabItem.HeaderProperty, new Binding("Title"));
   }

   /// <summary>
   /// Gets the view model that is contained by the container.
   /// </summary>
   /// <value>The view model.</value>
   public IViewModel ViewModel
   {
      get { return _logic.ViewModel; }
   }

   /// <summary>
   /// Occurs when a property on the container has changed.
   /// </summary>
   /// <remarks>
   /// This event makes it possible to externally subscribe to property changes of a <see cref="DependencyObject"/>
   /// (mostly the container of a view model) because the .NET Framework does not allows us to.
   /// </remarks>
   public event PropertyChangedEventHandler PropertyChanged;    

    /// <summary>
    /// Occurs when the view is loaded.
    /// </summary>
    event EventHandler<EventArgs> IView.Loaded
    {
        add { _viewLoaded += value; }
        remove { _viewLoaded -= value; }
    }

    /// <summary>
    /// Occurs when the view is unloaded.
    /// </summary>
    event EventHandler<EventArgs> IView.Unloaded
    {
        add { _viewUnloaded += value; }
        remove { _viewUnloaded -= value; }
    }

    /// <summary>
    /// Occurs when the data context has changed.
    /// </summary>
    event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs> IView.DataContextChanged
    {
        add { _viewDataContextChanged += value; }
        remove { _viewDataContextChanged -= value; }
    }
}
```

You would expect an abstract class here, but the designers (both Visual Studio and Expression Blend) cannot handle abstract base classes

## Using the class

The class can now be used the same as the `UserControl` class. For more information, see [UserControl explained]({{< relref "catel-mvvm/views/xaml/usercontrol.md" >}}).

