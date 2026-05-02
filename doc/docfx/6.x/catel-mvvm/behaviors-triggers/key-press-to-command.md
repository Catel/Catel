---
title: "KeyPressToCommand" 
---
Sometimes you need to handle a key press and convert it to a command. An excellent example is a ListBox that should respond to an `Ctrl + Enter` key press.

1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition:

```
<ListBox x:Name="listBox" ItemsSource="{Binding PersonCollection}" SelectedItem="{Binding SelectedPerson}">
    <i:Interaction.Behaviors>
        <catel:KeyPressToCommand Command="{Binding MyCommand}" Key="Enter" Modifiers="Ctrl" />
    </i:Interaction.Behaviors>
</ListBox>
```

