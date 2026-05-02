---
title: "MouseInfo" 
description: ""
---
1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition:

```
<ListBox ItemsSource="{Binding PersonCollection}" SelectedItem="{Binding SelectedPerson}">
    <i:Interaction.Behaviors>
        <catel:MouseInfo x:Name="personCollectionMouseInfo" />
    </i:Interaction.Behaviors>
</ListBox>
```

3) Now, it is easy to bind to the mouse information like this (textblock will become visible when the listbox is hovered):

```
<TextBlock Visibility="{Binding ElementName=personCollectionMouseInfo, Path=IsMouseOver, Converter={StaticResource BooleanToCollapsingVisibilityConverter}, ConverterParameter=false}" Text="Hovering listbox" />
```

