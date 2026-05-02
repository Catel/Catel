---
title: "Multilingual / Localization" 
description: ""
---
Making an application multilingual is a very common feature request nowadays. Therefore Catel provides the resources in several languages and provides theÂ *LanguageService* to give the developers full control over the translation process in their applications.

## Setting up the LanguageService

### Setting cultures

By default theÂ `LanguageService` will use the current UI culture to retrieve the right language values. These can easily be customized:

```
var dependencyResolver = this.GetDependencyResolver();
var languageService = dependencyResolver.Resolve<ILanguageService>();
Â 
languageService.PreferredCulture = new CultureInfo("nl-NL");
languageService.FallbackCulture = new CultureInfo("en-US");
```

### Registering custom language sources

In order to customize the language sources, custom language sources can be registered via theÂ *RegisterLanguageSource*Â method.

The code below shows how to add a newÂ *LanguageResourceSource*Â which represents a resource file in a specific assembly:

```
var dependencyResolver = this.GetDependencyResolver();
var languageService = dependencyResolver.Resolve<ILanguageService>();
Â 
// Create source for assembly MyApplication where the Resources.resx is located in the Properties folder
var resourcesSource = new LanguageResourceSource("MyApplication", "MyApplication.Properties", "Resources");
languageService.RegisterLanguageSource(resourcesSource );
Â 
// Create source for assembly MyApplication where the Exceptions.resx is located in the Properties folder
var exceptionsSource = new LanguageResourceSource("MyApplication", "MyApplication.Properties", "Exceptions");
languageService.RegisterLanguageSource(exceptionsSource );
```

TheÂ *LanguageService*Â will now automatically query these sources for the translations.

## Using the LanguageService

To use theÂ `LanguageService`, retrieve it via theÂ `DependencyResolver` (or let it be injected) and use the provided methods. The example below retrieves theÂ *WarningTitleÂ *resource string in theÂ *PreferredCulture*. If theÂ resource cannot be found in theÂ *PreferredCulture*, it will be retrieved for theÂ *FallbackCulture*. If that cannot be found, it will returnÂ *null*.

```
var dependencyResolver = this.GetDependencyResolver();
var languageService = dependencyResolver.Resolve<ILanguageService>();

var warningTitle = languageService.GetString("WarningTitle");
```

## Using the LanguageService in XAML

To use theÂ *LanguageServiceÂ *in XAML, Catel provides the markup extensions.

### Using theÂ LanguageBinding in

To use theÂ *LanguageBinding* markup extension, use the following code:

```
<TextBlock Text="{markup:LanguageBinding WarningTitle}" />
```

### Using the LanguageBinding in Windows Phone

Since Windows Phone does not support markup extensions, a customÂ *MarkupExtension* implementation is used in Catel. This requires a little difference in the usage of the markup extension:

```
<TextBlock Text="{markup:LanguageBinding ResourceName=WarningTitle}" />
```

## Implementing custom LanguageService (from database)

Implementing a customÂ *LanguageService* consists of several steps which are described below.

{{% notice warning %}}
Note that this implementation queries the database for each translation. It is best to read all translations into memory at once to improve performance
{{% /notice %}}

### Creating a custom ILanguageSource implementation

First of all, we need to implement a customized language source to allow the custom service to know what source to read for translations:

```
public class DbLanguageSource : ILanguageSource
{
    public DbLanguageSource(string connectionString)
    {
        Argument.IsNotNullOrWhitespace(() => connectionString);

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

Below is a custom implementation of theÂ *LanguageService*. Note that we only have to derive a single method to fully customize the implementation:

```
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

To enable the customÂ *DbLanguageService*, it must be registered in the *ServiceLocator*:

```
var serviceLocator = ServiceLocator.Default;
Â 
var dbLanguageService = new DbLanguageService();
Â 
var dbLanguageSource = new DbLanguageSource("myConnectionString");
dbLanguageService.RegisterLanguageSource(dbLanguageSource);
Â 
serviceLocator.RegisterInstance<ILanguageService>(dbLanguageService);
```

