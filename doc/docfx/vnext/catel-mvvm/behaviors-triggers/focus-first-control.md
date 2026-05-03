# FocusFirstControl

The Focus behavior is powerful, but sometimes you just need to focus the first control on a window or control. This can be done by using the FocusFirstControl behavior instead. This behavior will focus the first control on a window or control and has only one property: `FocusParentFirst`.

Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

## Focus when the control is loaded

The easiest and default method is to focus the first control. The parent is also focused by default (just in case if it does not have any focus):

```
<Window ...>
    <i:Interaction.Behaviors>
        <catel:FocusFirstControl />
    </i:Interaction.Behaviors>
</Window>
```

