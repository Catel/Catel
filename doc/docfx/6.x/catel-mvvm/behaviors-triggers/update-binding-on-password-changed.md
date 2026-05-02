---
title: "UpdateBindingOnPasswordChanged" 
---
The `UpdateBindingOnPasswordChanged` is a very useful behavior which allows to bind the `Password` property of the `PasswordBox` Control. Use it, it's really simple.

Â 1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition:

```
<PasswordBox>
   <i:Interaction.Behaviors>
         <catel:UpdateBindingOnPasswordChanged Password="{Binding Password, Mode=TwoWay}" />
   </i:Interaction.Behaviors>
</PasswordBox>
```

