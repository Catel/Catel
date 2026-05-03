---
title: "Running code at design-time" 
---
Sometimes one needs to run code during design-time. A good example is to register a custom `LanguageResourceSource` in the `LanguageService` to show translations in the designer. Unfortunately it is not as easy as putting some code in the code-behind to get this working.

Starting with Catel 4.3, calling `CatelEnvironment.IsInDesignTime` will automatically invoke *DesignTimeHelper.InitializeDesignTime* if in design mode. This means you only have to add classes deriving from `DesignTimeInitializer` and they will automatically be invoked when any code is checking if it is currently running inside a designer context (e.g. `LanguageBinding`)

## Introducing the DesignTimeCode attribute

To allow users to run code in the designer (Visual Studio or Blend), Catel introduces the `DesignTimeCodeAttribute`. This is an assembly-wide attribute which will specify the class to create at design time. This means that it is possible to create multiple attributes. Below is an example of the usage of the attribute:

```
[assembly: DesignTimeCode(typeof(WpfApplication.Catel.DesignTimeLanguageService))]
[assembly: DesignTimeCode(typeof(WpfApplication.Catel.DesignTimeServiceLocator))]
```

When the attribute is found by Catel, it will automatically construct the types specified in the attribute. This will allow the constructor to execute any code during design time.

## Introducing the DesignTimeInitializer

The `DesignTimeCodeAttribute` contains the types that are constructed during design-time. Although the reflection in Catel is protected by only checking these arguments at design-time, it cannot be guaranteed for other systems. Therefore Catel also provides the `DesignTimeInitializer` base class. This is a base class that checks whether the type being constructed is actually running inside a design tool.

Below is an example of the usage, which registers custom language resource sources in the language service. This allows real-time updates of the `LanguageService` in the designer.

```csharp
public class DesignTimeLanguageService : Catel.DesignTimeInitializer
{
    protected override void Initialize()
    {
        // Design-time initializers cannot use constructor injection because they are
        // instantiated by Catel at design time. Use the IoC container directly here.
        var languageService = IoCContainer.ServiceProvider?.GetService<ILanguageService>();
        if (languageService is null)
        {
            return;
        }

        languageService.CacheResults = false;

        var resourcesSource = new LanguageResourceSource("WpfApplication.Catel", "WpfApplication.Catel.Properties", "Resources");
        languageService.RegisterLanguageSource(resourcesSource);
    }
}
```

