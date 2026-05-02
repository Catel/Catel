---
title: "Improving performance by defining the view model grid manually" 
---
Catel wraps the content of each user control in a grid. This grid is the actual view model container as can be read in the [advanced documentation]({{< relref "catel-mvvm/views/xaml/advanced/usercontrol-under-the-hood.md" >}}). This wrapper is very convenient, but does have (little) impact on the performance and visual state management. To prevent Catel to wrap the control for you, there are a few things you can do.

## Creating the grid manually

The easiest way is to create the grid manually. The default implementation of the `IViewModelWrapperService` checks if the direct child control is a grid and has a specific name `__catelInnerWrapper`. To prevent Catel from creating the wrapper, simply specify the name on the root grid.

Note that Catel will override the `DataContext` binding of this grid to reflect the view model.

## Customizing the `IViewModelWrapperService`

Another way to prevent Catel from wrapping the grids is to implement the `IViewModelWrapperService` yourself.

Note that we recommend to create the grid manually since that is highly likely to have less side-effects

