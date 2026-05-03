---
title: "Multilingual / Localization" 
---
Making an application multilingual is a common feature request nowadays. Therefore Catel provides the resources in several languages and provides the *LanguageService* to give the developers full control over the translation process in their applications.

## Setting up the LanguageService

### Setting cultures

By default the `LanguageService` will use the current UI culture to retrieve the right language values. Inject `ILanguageService` via the constructor and customize:

```csharp
private readonly ILanguageService _languageService;

public MyViewModel(IServiceProvider serviceProvider, ILanguageService languageService)
    : base(serviceProvider)
{
    _languageService = languageService;
}
```

```csharp
_languageService.PreferredCulture = new CultureInfo("nl-NL");
_languageService.FallbackCulture = new CultureInfo("en-US");
```

### Registering custom language sources

In order to customize the language sources, custom language sources can be registered via the *RegisterLanguageSource* method.

The code below shows how to add a new *LanguageResourceSource* which represents a resource file in a specific assembly:

```csharp
// Create source for assembly MyApplication where the Resources.resx is located in the Properties folder
var resourcesSource = new LanguageResourceSource("MyApplication", "MyApplication.Properties", "Resources");
_languageService.RegisterLanguageSource(resourcesSource);

// Create source for assembly MyApplication where the Exceptions.resx is located in the Properties folder
var exceptionsSource = new LanguageResourceSource("MyApplication", "MyApplication.Properties", "Exceptions");
_languageService.RegisterLanguageSource(exceptionsSource);
```

The *LanguageService* will now automatically query these sources for the translations.

## Using the LanguageService

To use the `LanguageService`, inject it via the constructor and use the provided methods. The example below retrieves the *WarningTitle* resource string in the *PreferredCulture*. If the resource cannot be found in the *PreferredCulture*, it will be retrieved for the *FallbackCulture*. If that cannot be found, it will return *null*.

```csharp
var warningTitle = _languageService.GetString("WarningTitle");
```

## Using the LanguageService in XAML

To use the *LanguageService* in XAML, Catel provides the markup extensions.

### Using the LanguageBinding

To use the *LanguageBinding* markup extension, use the following code:

```
<TextBlock Text="{markup:LanguageBinding WarningTitle}" />
```

## Implementing custom LanguageService (from database)

Implementing a custom *LanguageService* consists of several steps which are described below.

Note that this implementation queries the database for each translation. It is best to read all translations into memory at once to improve performance

### Creating a custom ILanguageSource implementation

First of all, we need to implement a customized language source to allow the custom service to know what source to read for translations:

```csharp
public class DbLanguageSource : ILanguageSource
{
    public DbLanguageSource(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; private set; }

    public string GetSource()
    {
        return ConnectionString;
    }
}
```

### Creating a custom DbLanguageService

Below is a custom implementation of the *LanguageService*. Note that we only have to derive a single method to fully customize the implementation:

```csharp
public class DbLanguageService : LanguageService
{
    protected override string GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo)
    {
        var connectionString = languageSource.GetSource();
        using (var dbConnection = new SqlConnection(connectionString))
        {
            dbConnection.Open();

            var sqlCommand = dbConnection.CreateCommand();
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = @"SELECT [Name] FROM [Translations] WHERE [ResourceName] = @ResourceName AND [CultureName] = @CultureName";
            sqlCommand.Parameters.Add(new SqlParameter("ResourceName", resourceName));
            sqlCommand.Parameters.Add(new SqlParameter("CultureName", cultureInfo.ThreeLetterISOLanguageName));

            var translation = sqlCommand.ExecuteScalar() as string;
            if (!string.IsNullOrWhiteSpace(translation))
            {
                return translation;
            }
        }

        // Resource not found, fall back to base if you like, or simply return null
        return base.GetString(languageSource, resourceName, cultureInfo);
    }
}
```

### Enabling the custom DbLanguageService

To enable the custom *DbLanguageService*, register it in the service collection:

```csharp
services.AddSingleton<ILanguageService>(sp =>
{
    var dbLanguageService = new DbLanguageService();
    var dbLanguageSource = new DbLanguageSource("myConnectionString");
    dbLanguageService.RegisterLanguageSource(dbLanguageSource);
    return dbLanguageService;
});
```


