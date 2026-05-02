---
title: "Validation via data annotations" 
description: ""
weight: 20
---
{{% notice info %}}
The ViewModelBase derives from ModelBase, thus all information here also applies to the ViewModelBase
{{% /notice %}}

Data annotations are validation when the specific property is set. For example, when a property FirstName is set, all the data annotations on the FirstName property are validated.

## Decorating properties with data annotations

Decorating properties is very simple. For example, to make a property mandatory, use the following definition (note the Required attribute):

```
/// <summary>
/// Gets or sets the middle name.
/// </summary>
[Required]
public string MiddleName
{
    get { return GetValue<string>(MiddleNameProperty); }
    set { SetValue(MiddleNameProperty, value); }
}

/// <summary>
/// Register the property so it is known in the class.
/// </summary>
public readonly PropertyData MiddleNameProperty = RegisterProperty("MiddleName", typeof(string), string.Empty);
```

For more information about data annotations, read the [official MSDN documentation](http://msdn.microsoft.com/en-us/library/dd901590(v=vs.95).aspx)Â .

