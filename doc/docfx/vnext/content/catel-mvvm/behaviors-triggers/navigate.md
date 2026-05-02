---
title: "Navigate" 
description: ""
---
{{% notice warning %}}
Note that this behavior is only available for WPF
{{% /notice %}}

The `Hyperlink` control in WPF is very powerful, but it is hard to make them work outside pages.

Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

To execute the NavigateUrl, simply use the behavior as shown below:

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

{{% notice info %}}
Another alternative is to use the `LinkLabel` control from [Orc.Controls](https://github.com/wildgums/orc.controls)
{{% /notice %}}

