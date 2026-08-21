---
title: "DelayBindingUpdate" 
---
Sometimes, a binding update should be delayed for performance reasons. This is possible using the `DelayBindingUpdate` behavior. This behavior modifies the binding mode to explicit and internally watches for property changes. If the bound dependency property changes, the behavior will wait for the time to pass and then update. If the value changes again within the timeframe, the timer is reset (so you will not get "double" updates).

1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition. This example will delay the update of the SelectedItem binding with 100 milliseconds:

```
<ListBox x:Name="listBox" ItemsSource="{Binding PersonCollection}" SelectedItem="{Binding SelectedPerson}">
  <i:Interaction.Behaviors>
    <catel:DelayBindingUpdate PropertyName="SelectedItem" UpdateDelay="100" />
  </i:Interaction.Behaviors>
</ListBox>
```

