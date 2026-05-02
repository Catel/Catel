---
title: "SelectTextOnFocus" 
---
The `SelectTextOnFocus` behavior makes it easy to select all text when a `TextBox` receives the focus.

1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition:

```
<TextBox Text={Binding Amount}">
    <i:Interaction.Behaviors>
        <catel:SelectTextOnFocus />
    </i:Interaction.Behaviors>
</TextBox>
```

