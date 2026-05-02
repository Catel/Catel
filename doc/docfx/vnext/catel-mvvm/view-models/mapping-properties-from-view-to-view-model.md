---
title: "Mapping properties from view to view model" 
---
Sometimes a view (for example, a user control) contains additional properties besides the `DataContext` to interact with the view model. By default, it is hard to implement this in an MVVM sccenario, but Catel solves this issue using the `ViewToViewModel` attribute.

This attribute automatically keeps track of changes in both the view and the view model and this way, a control can have several properties and still implement MVVM.

## Example implementation

The example below shows how the `MapCenter` is a dependency property on the control. It automatically maps the property to the `ViewModel.MapCenter` property.

```
public partial class MyControl : UserControl
{
    static MyControl()
    {
        typeof(MyControl).AutoDetectViewPropertiesToSubscribe();
    }
Â 
    public MyControl()
    {
        InitializeComponent();
    }
Â 
    [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewModelToView)]
    public GeoCoordinate MapCenter
    {
        get { return (GeoCoordinate) GetValue(MapCenterProperty); }
        set { SetValue(MapCenterProperty, value); }
    }
Â 
    // Using a DependencyProperty as the backing store for MapCenter.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MapCenterProperty = DependencyProperty.Register("MapCenter", typeof (GeoCoordinate),
        typeof (MyControl), new PropertyMetadata(null, (sender, e) => ((MyControl) sender).UpdateMapCenter()));
Â 
    private void UpdateMapCenter()
    {
        map.SetView(ViewModel.MapCenter, ViewModel.ZoomLevel);
    }
Â 
    public new MainPageViewModel ViewModel
    {
        get { return base.ViewModel as MainPageViewModel; }
    }
}
```

## Important note starting with Catel 4.0

Starting with 4.0, Catel no longer subscribes to dependency properties automatically.Â It is best to let Catel only subscribe to the properties that it should (for the best performance). To do so, use theÂ `IViewPropertySelector.AddPropertyToSubscribe`Â method to add properties:

```
var serviceLocator = ServiceLocator.Default;
var viewPropertySelector = serviceLocator.ResolveType<IViewPropertySelector>();

viewPropertySelector.AddPropertyToSubscribe("MyProperty", typeof(MyView));
```

In most cases, the only reason to subscribe to property changes is because of theÂ `ViewToViewModel` attribute. If that is the case, it is best to use the extension methodÂ `AutoDetectViewPropertiesToSubscribe`Â in the static constructor of the view:

```
static MyView()
{
    typeof(MyView).AutoDetectViewPropertiesToSubscribe();
}
```

## Mapping types

Catel supports the following mapping types using the `ViewToViewModelMappingType` enum.

Type

Description

TwoWayDoNothing

Two way, which means that either the view or the view model will update the values of the other party as soon as they are updated.

When this value is used, nothing happens when the view model of the view changes. This way, it might be possible that the values of the view and the view model are different. The first one to update next will update the other.

TwoWayViewWins

Two way, which means that either the view or the view model will update the values of the other party as soon as they are updated.

When this value is used, the value of the view is used when the view model of the view is changed, and is directly transferred to the view model value

TwoWayViewModelWins

Two way, which means that either the view or the view model will update the values of the other party as soon as they are updated.

When this value is used, the value of the view model is used when the view model of the view is changed, and is directly transferred to the view value.

ViewToViewModel

The mapping is from the view to the view model only.

ViewModelToView

The mapping is from the view model to the view only.

