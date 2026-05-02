---
title: 'Catel 6.0'
description: ""
weight: 600
---
This guide describes how to update your code to be fully compatible with Catel 6.0.

{{% notice warning %}}
This guide assumes that you are coming from Catel 5.12. If not, please read that guide first.
{{% /notice %}}

Encountered issues while upgrading to this version? Add them here to help out others!

## Generic property registrations

{{% notice info %}}
It's recommended to use `Catel.Fody`. This tool will automatically detect the version of Catel being used and use the correct syntax to register properties.
{{% /notice %}}

If not using `Catel.Fody`, the property registrations of the (view) models has changed to be generic by default. This is done to improve performance of the underlying property bag.

### Migrate to (faster) generic property registration

Refactor the property registration to the generic form:

```
public string FirstName
{
    get { return GetValue<string>(nameof(FirstName)); }
    protected set { SetValue(nameof(FirstName), value); }
}

public static readonly IPropertyData FirstNameProperty = RegisterProperty<string>(nameof(FirstName));
```

### Stick with (slower) non-generic property registration

Rename the `RegisterProperty` method to `RegisterPropertyNonGeneric`:

```
public string FirstName
{
    get { return GetValue<string>(nameof(FirstName)); }
    protected set { SetValue(nameof(FirstName), value); }
}

public static readonly IPropertyData FirstNameProperty = RegisterPropertyNonGeneric(nameof(FirstName), typeof(string), null, null, false, true, true);
```

## IPleaseWaitService renamed to IBusyIndicatorService

To be more compliant with other software components available, the `PleaseWaitService` has been renamed to `BusyIndicatorService`.

