---
title: "Property bindings" 
---
Property bindings are very important in the MVVM pattern. The binding system in Catel will automatically map properties when the binding system is used.

## Binding properties one way

To bind properties one way, use the code below.

### Android

```
protected override void AddBindings(BindingContext bindingContext, IViewModel viewModel)
{
    var vm = (MainViewModel) viewModel;

    bindingContext.AddBinding(() => vm.FirstName, () => _firstNameEditText.Text, BindingMode.OneWay);
}
```

### iOS

iOS not yet documented

## Binding properties two way

To bind properties two way, use the code below.

### Android

```
protected override void AddBindings(BindingContext bindingContext, IViewModel viewModel)
{
    var vm = (MainViewModel) viewModel;

    bindingContext.AddBinding(() => vm.FirstName, () => _firstNameEditText.Text).AddTargetEvent("TextChanged");
}
```

Note that you need to use the *AddTargetEvent* to allow two way binding in Android

### iOS

iOS not yet documented

## Binding properties with a converter

Converters are a well-known topic in MVVM. Catel supports the use of converters in the binding system. The example below will convert an integer (*vm.Counter*) to a string with a format to *"{0} clicks!"*. The converter will automatically be instantiated using the *TypeFactory*. Note that using converters in Catel support both *TwoWay* bindings and *ConverterHelper.UnsetValue* to prevent any changes in the binding system.

### Android

```
protected override void AddBindings(BindingContext bindingContext, IViewModel viewModel)
{
    var vm = (MainViewModel) viewModel;

    bindingContext.AddBindingWithConverter<ClicksConverter>(() => vm.Counter, () => _testButton.Text, BindingMode.OneWay);
}
```

### iOS

iOS not yet documented


