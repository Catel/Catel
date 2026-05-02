---
title: "Type cache" 
description: ""
---
The `TypeCache` is the centralized cache by Catel for type information. It should be used by any class to gather type information since most of the calls are being cached by Catel.

## Getting types without versioning

Sometimes you know what type to get and what assembly it is living in. However, you don't want to be version-dependent by specifying the fully qualified assembly name. By using the `TypeCache.GetType` method, it is possible to get a type by only the assembly name (e.g. `Catel.Core`) and the type name (e.g. `Catel.Data.ObservableObject`).

```
var type = TypeCache.GetType("Catel.Data.ObservableObject", "Catel.Core");
```

{{% notice warning %}}
More documentation should be written in the future
{{% /notice %}}

