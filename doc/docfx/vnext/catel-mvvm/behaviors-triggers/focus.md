---
title: "Focus" 
---
To set the focus on a UI element, one must write code in the code-behind. With the `Focus` behavior, this is no longer necessary. This behavior sets the focus only once on the first time the associated object is loaded.

Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

## Focus when the control is loaded

The easiest and default method is to set the focus when the associated control is loaded. In WPF, this is immediately when the control is focused.

```
<ListBox ItemsSource="{Binding PersonCollection}" SelectedItem="{Binding SelectedPerson}">
    <i:Interaction.Behaviors>
        <catel:Focus />
    </i:Interaction.Behaviors>
</ListBox>
```

## Focus when an event occurs

It is possible to set the focus when a specific event occurs. For example, when the layout root gets a MouseEnter event, the focus must be set on a specific control. This can be done via the following code:

```
<ListBox ItemsSource="{Binding PersonCollection}" SelectedItem="{Binding SelectedPerson}">
    <i:Interaction.Behaviors>
        <catel:Focus FocusMoment="Event" Source="{Binding ElementName=layoutRoot}" EventName="MouseEnter" />
    </i:Interaction.Behaviors>
</ListBox>
```

## Focus when a property changes

It is possible to set the focus when a specific property changes. For example, when a value is set, the focus must move on to a new control. This can be done via the following code:

```
<ListBox ItemsSource="{Binding PersonCollection}" SelectedItem="{Binding SelectedPerson}">
    <i:Interaction.Behaviors>
        <catel:Focus FocusMoment="PropertyChanged" Source="{Binding }" PropertyName="MyProperty" />
    </i:Interaction.Behaviors>
</ListBox>
```

