---
title: "AutoScroll" 
description: ""
---
TheÂ `AutoScroll` behavior automatically scrolls to a specific direction when theÂ `ItemsSource` of anÂ `ItemsControl` changes.

1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Add behavior

```
<ListBox ItemsSource="{Binding LogEntries}">
    <i:Interaction.Behaviors>
        <catel:AutoScroll ScrollDirection="Bottom" ScrollTreshold="10" />
    </i:Interaction.Behaviors>
</ListBox>
```

TheÂ `ScrollDirection` determines the direction (`Top` or `Bottom`).

TheÂ `ScrollTreshold` allows the treshold of the real offset (to determine whether auto scroll should be enabled). For example, when the user is manually scrolling, this behavior will pause.

