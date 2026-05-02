---
title: "Naming conventions" 
---
Some services in Catel support naming conventions. For example, the `IViewLocator` and `IViewModelLocator` allow naming conventions to prevent a user from having to register all views and view models. Internally, the naming conventions are resolved using the `NamingConvention` helper class. This part of the documentation explains the possible constants in naming conventions.

## [AS] constant

The [AS] constant will be replaced by the assembly name. For example, the following naming convention:

```
[AS].Views
```

in assembly *Catel.Examples* will be resolved as:

```
Catel.Examples.Views
```

## [VM] constant

The [VM] constant will be replaced by the name of the view model without the *ViewModel* postfix. For example, the following naming convention:

```
[AS].ViewModels.[VW]ViewModel
```

in assembly *Catel.Examples* and for type *Catel.Examples.ViewModels.MyViewModel* will be resolved as:

```
Catel.Examples.ViewModels.MyViewModel
```

## [VW] constant

The [VW] constant will be replaced by the name of the view without the *View*, *Control*, *Page* or *Window* postfixes. For example, the following naming convention:

```
[AS].Views.[VM]View
```

in assembly *Catel.Examples* and for type *Catel.Examples.Views.MyView* will be resolved as:

```
Catel.Examples.Views.MyView
```

## [UP] constant

Sometimes it is not possible to use the [AS] constant because the assembly name is not used in the namespace. For example, for an application called *PersonApplication* where the client assembly is *PersonApplication.Client*, the root namespace will still be *PersonApplication*. Therefore, it is recommend to use the [UP] constant for this situation.

The [UP] constant will move the namespaces up by one step. It automatically detects the right separator (\\ (backslash), / (slash), . (dot) and | (pipe) are supported).

The following naming convention:

```
[UP].Views.[VM]View
```

for type *Catel.Examples.ViewModels.MyViewModel* will be resolved as:

```
Catel.Examples.Views.MyView
```

## [CURRENT] constant

Some people prefer to put classes into the same namespace (such as views and view models).

The [CURRENT] constant will use the same namespace.

The following naming convention:

```
[CURRENT].[VM]View
```

for typeÂ *Catel.Examples.MyViewModel*Â will be resolved as:

```
Catel.Examples.MyView
```

