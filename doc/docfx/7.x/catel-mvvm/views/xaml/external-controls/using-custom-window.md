---
title: "Using a custom window" 
---
In this part of the documentation, the `RadWindow` of Telerik will be used as an example on how to create a `WindowBase` that behaves like the `DataWindow`.

## Creating the base class with behavior

The first thing to do is to create a new base class that accepts a view model type argument. In this example, we will call it `WindowBase` (to make it as "external control company independent" as possible). Below is the code for the window definition. The downside of xaml based applications is that you cannot derive from controls or windows that have a partial class defined in xaml. Therefore, all controls and code must be initialized via code as shown, in the code below.

Because the `RadWindow` of Telerik does not close the window when the `DialogResult` is set, this window subscribes to the `ViewModelClosed` event to close the window

```
/// <summary>
/// Base class for a window with the Catel mvvm behavior.
/// </summary>
public class Window : RadWindow, IDataWindow
{
    private readonly WindowLogic _logic;

    private event EventHandler<EventArgs> _viewLoaded;
    private event EventHandler<EventArgs> _viewUnloaded;
    private event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs> _viewDataContextChanged;

    public Window()
        : this(null)
    {

    }
    public Window(IViewModel viewModel)
    {
        _logic = new WindowLogic(this, null, viewModel);
        _logic.ViewModelChanged += (sender, e) => ViewModelChanged.SafeInvoke(this, e);
        _logic.PropertyChanged += (sender, e) => PropertyChanged.SafeInvoke(this, e);

        Loaded += (sender, e) => _viewLoaded.SafeInvoke(this);
        Unloaded += (sender, e) => _viewUnloaded.SafeInvoke(this);
        this.AddDataContextChangedHandler((sender, e) => _viewDataContextChanged.SafeInvoke(this, new Catel.MVVM.Views.DataContextChangedEventArgs(e.OldValue, e.NewValue)));

        // Because the RadWindow does not close when DialogResult is set, the following code is required
        ViewModelChanged += (sender, e) => OnViewModelChanged();

        // Call manually the first time (for injected view models)
        OnViewModelChanged();

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SetBinding(RadWindow.HeaderProperty, new Binding("Title"));
    }

    public IViewModel ViewModel
    {
        get { return _logic.ViewModel; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler<EventArgs> ViewModelChanged;

    event EventHandler<EventArgs> IView.Loaded
    {
        add { _viewLoaded += value; }
        remove { _viewLoaded -= value; }
    }

    event EventHandler<EventArgs> IView.Unloaded
    {
        add { _viewUnloaded += value; }
        remove { _viewUnloaded -= value; }
    }

    event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs> IView.DataContextChanged
    {
        add { _viewDataContextChanged += value; }
        remove { _viewDataContextChanged -= value; }
    }

    private void OnViewModelChanged()
    {
        if (ViewModel != null && !ViewModel.IsClosed)
        {
            ViewModel.Closed += ViewModelClosed;
        }
    }

    private void ViewModelClosed(object sender, ViewModelClosedEventArgs e)
    {
        Close();
    }
}
```

You would expect an abstract class here, but the designers (both Visual Studio and Expression Blend) cannot handle abstract base classes

## Handling Close Event
Since the latest upgrade to Catel 5.x, the [UIVisualizerService]({{< relref "catel-mvvm/services/ui-visualizer-service.md" >}}) has been modified in terms of how `Close` events are handled. Currently, a non-generic `EventHandler` is utilized; this would present a problem if a window other than `DataWindow` is utilized and the `Close` event handler is generic; a simple cast from `EventHandler` to `EventHandler<>` is not possible without reflection. For our example here, the `RadWindow` utilizes `EventHandler<WindowClosedEventArgs>`. In order to be able to utilize `UIVisualizerService` without any exception, the `HandleCloseSubscription` will need to be overridden. See example below:

```
public class CustomUIVisualService : UIVisualizerService, ICustomUIVisualizerService
{
  private readonly IViewLocator _viewLocator;

  public CustomUIVisualService(IViewLocator viewLocator) 
      : base(viewLocator)
  {
      this._viewLocator = viewLocator;
  }        

  protected override void HandleCloseSubscription(object window, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
  {
      var eventInfo = window.GetType().GetEvent("Closed");
      var addMethod = eventInfo?.AddMethod;

      if (addMethod != null)
      {
          EventHandler<WindowClosedEventArgs> eventHandler = null;
          void Closed(object s, EventArgs e)
          {
              if (!ReferenceEquals(window, s))
              {
                  // Fix for https://github.com/Catel/Catel/issues/1074
                  return;
              }

              bool? dialogResult;
              PropertyHelper.TryGetPropertyValue(window, "DialogResult", out dialogResult);

              try
              {
                  completedProc(this, new UICompletedEventArgs(data, isModal ? dialogResult : null));
              }
              finally
              {
                  var removeMethod = eventInfo.RemoveMethod;
                  if (removeMethod != null)
                  {
                      removeMethod.Invoke(window, new object[] { eventHandler });
                  }
              }
         }

        eventHandler = Closed;
        addMethod.Invoke(window, new object[] { eventHandler });
    }
  }
}
```

We have basically created a new class named `CustomUIVisualizerService` which inherits from `UIVisualizerService` and then have overridden the `HandleCloseSubscription`. If you peek into the base method definition you will see that the only change is from `EventHandler` to `EventHandler<WindowClosedEventArgs>`.

The `ICustomUIVisualizerService` interface is only a convenience interface to allow for service injection:

```
public interface ICustomUIVisualizerService : IUIVisualizerService
{
}
```

this will need to be registered with the service collection to be utilized correctly:

```csharp
services.AddSingleton<ICustomUIVisualizerService, CustomUIVisualizerService>();
```

## Using the class

The class can now be used the same as the `DataWindow` class. For more information, see [Window and DataWindow]({{< relref "catel-mvvm/views/xaml/window-and-datawindow.md" >}}).

