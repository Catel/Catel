---
title: 'Catel 5.4'
---
This guide describes how to update your code to be fully compatible with Catel 5.4.

This guide assumes that you are coming from Catel 5.x. If not, please read that guide first.

Encountered issues while upgrading to this version? Add them here to help out others!

## Updated converters

The behavior of converters have been changed for some converters. The converter parameter will now also serve as an "inverter". For example,
the `BooleanToCollapsingVisibilityConverter` now behaves like this:

```
    // Show the text box when `IsEnabled` is `true`
    <TextBox Visibility="{catel:BooleanToCollapsingVisibilityConverter IsEnabled}" />
    <TextBox Visibility="{catel:BooleanToCollapsingVisibilityConverter IsEnabled, ConverterParameter=False}" />

    // Show the text box when `IsEnabled` is `false`
    <TextBox Visibility="{catel:BooleanToCollapsingVisibilityConverter IsEnabled, ConverterParameter=True}" />
```

