---
title: 'Catel 5.0'
description: ""
weight: 500
---
This guide describes how to update your code to be fully compatible with Catel 5.0.

{{% notice warning %}}
Catel v5 contains a lot of breaking changes, not all of them cause compiler errors but change core behavior. It's important to check out the guide below **before** updating to Catel v5.
{{% /notice %}}

{{% notice warning %}}
This guide assumes that you are coming from Catel 4.5. If not, please read that guide first.
{{% /notice %}}

Encountered issues while upgrading to this version? Add them here to help out others!

## Platform support changes

Added support for the following platforms:

* .NET 4.7

Removed support for the following platforms:

* .NET 4.0
* Silverlight 5
* Windows Phone 8.0 (Silverlight)
* Windows Phone 8.1 (Silverlight)
* Windows Phone 8.1 (Runtime)
* Windows 8.1 (Runtime)

## Moved extensions

The following extensions have been moved to a new repository:

* Catel.Extensions.Controls (moved to [Orc.Controls](https://github.com/wildgums/orc.controls) & [Orchestra](https://github.com/wildgums/orchestra))
* Catel.Extensions.DynamicObjects (moved to [Orc.DynamicObjects](https://github.com/wildgums/orc.dynamicobjects))
* Catel.Extensions.EntityFramework (moved to [Orc.EntityFramework](https://github.com/wildgums/orc.entityframework))
* Catel.Extensions.FluentValidation (moved to [Orc.FluentValidation](https://github.com/wildgums/orc.fluentvalidation))
* Catel.Extensions.Memento (moved to [Orc.Memento](https://github.com/wildgums/orc.memento))
* Catel.Extensions.Prism5 (moved to [Orc.Prism](https://github.com/wildgums/orc.prism))
* Catel.Extensions.Prism6 (moved to [Orc.Prism](https://github.com/wildgums/orc.prism))

## Removed extensions

The following extensions have been removed:

* Catel.Extensions.CSLA
* Catel.Extensions.Data
* Catel.Extensions.Interception
* Catel.Extensions.MVC4
* Catel.Extensions.MVC5
* Catel.Extensions.Prism4
* Catel.Extensions.Wcf.Server

## Catel.Fody

{{% notice warning %}}
Note that if you are using Catel.Fody, you *must* update to at least 2.17 to support Catel 5.
{{% /notice %}}

## Catel.Core

### CommandLineHelper

`CommandLineHelper` has been removed. Use [Orc.CommandLine](https://github.com/wildgums/orc.commandline) instead.

### DynamicEventListener

`DynamicEventListener` has been removed. Use `WeakEventListener` instead.

### ModelBase

The `ModelBase` has been greatly simplified to decrease the memory print and improve performance. The class is split up into separate classes:

- ModelBase => takes care of basic serialization and property bag registrations
- ValidatableModelBase => adds validation
- ChildAwareModelBase => contains the child event subscriptions logic (which is very costly)
- SavableModelBase => adds `Save` and `Load` methods 
- ComparableModelBase => contains the equality comparer members. This means the `ModelBase` itself does no longer support equality checks based on the property values

## Catel.MVVM

### XAML namespace changes

The xaml namespace has changed from `http://catel.codeplex.com` to `http://schemas.catelproject.com`

### CountCollapsedConverter

`CountCollapsedConverter` has been removed. Use `CollectionToVisibilityConverter` instead.

### EffectsHelper

`EffectsHelper` has been removed. No replacement is available.

### ViewModelBase

The `ViewModelBase` has been simplified. It now derives from `ValidatingModelBase` (so still supports validation). The following features / members have been removed:

* `HasDirtyModels` has been removed
* Validation summaries (use the `ValidationContext` instead)

### VisualTargetPresentationSource

`VisualTargetPresentationSource` has been removed. No replacement is available.

