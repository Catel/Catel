---
title: "NumericTextBox" 
description: ""
---
The `NumericTextBox` behavior makes it easy to allow specific numeric input on a TextBox.

1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition:

```
<TextBox Text={Binding Amount}">
    <i:Interaction.Behaviors>
        <catel:NumericTextBox />
    </i:Interaction.Behaviors>
</TextBox>
```

{{% notice info %}}
Use the properties on the behavior to customize the behavior to your needs
{{% /notice %}}

