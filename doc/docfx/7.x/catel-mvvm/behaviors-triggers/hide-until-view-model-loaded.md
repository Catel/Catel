---
title: "HideUntilViewModelLoaded" 
---
The `HideUntilViewModelLoaded` hides (`Visibility.Collapsed`) any view model container (`IViewModelContainer`) when it does not have a valid view model. This is a great way to hide lazy-loaded views that should only be visible when they contain an actual view model.

Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

## Focus when the control is loaded

The easiest and default method is to focus the first control. The parent is also focused by default (just in case if it does not have any focus):

```
<local:MyUserControl ...>
    <i:Interaction.Behaviors>
        <catel:HideUntilViewModelLoaded />
    </i:Interaction.Behaviors>
</local:MyUserControl>
```

