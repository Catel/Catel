---
title: "Navigate" 
---
Note that this behavior is only available for WPF

The `Hyperlink` control in WPF is powerful, but it is hard to make them work outside pages.

Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

To execute the NavigateUrl, use the behavior as shown below:

```
<TextBlock>
    <Hyperlink NavigateUri="http://schemas.catelproject.com">
        <i:Interaction.Behaviors>
            <catel:Navigate />
        </i:Interaction.Behaviors>

        <TextBlock Text="The best MVVM Framework" />
    </Hyperlink>
</TextBlock>
```

Another alternative is to use the `LinkLabel` control from [Orc.Controls](https://github.com/wildgums/orc.controls)


