---
title: "UpdateBindingOnTextChanged" 
---
The `UpdateBindingOnTextChange` is a useful behavior which allows to delay a binding update on the `TextChanged` event of a `TextBox`. This way, it is possible to implement search boxes that only start a search after a specific time when no new key presses have occurred. For example, when a user types a new search string, and the user does not enter a new key for 500 ms, the binding is updated.

1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition. This example will update the binding after 500 ms where normally it would only occur when the user tabs out of the `TextBox`:

```
<TextBox Text="{Binding SearchParam, Mode=TwoWay}">
    <i:Interaction.Behaviors>
        <catel:UpdateBindingOnTextChanged UpdateDelay="500" />
    </i:Interaction.Behaviors>
</TextBox>
```

