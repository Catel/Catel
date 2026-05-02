# FocusOnKeyPress

Sometimes you need to handle a key press and then set the focus on an element.

1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition:

```
<TextBox x:Name="textBox">
    <i:Interaction.Behaviors>
        <catel:FocusOnKeyPress Key="F" Modifiers="Ctrl" />
    </i:Interaction.Behaviors>
</ListBox>
```

