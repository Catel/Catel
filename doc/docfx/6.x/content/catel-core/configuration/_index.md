---
title: "Configuration" 
description: ""
---
Catel makes it very easy to use configurations on all supported platforms.Â 

Below is a table to explain what technology is used per platform to retrieve and store configuration values.

Platform|Technology
---|---
.NET|ConfigurationManager.AppSettings
UWP|ApplicationData.Current.LocalSettings
PCL|Not supported

## Getting values from the configuration

To retrieve values from the configuration, use the following code:

```
var configurationService = new ConfigurationService();
var mySetting = configurationService.GetValue<int>("mySetting", 42);
```

The code above will retrieve the values from the configuration. If the configuration value does not exist, it will return *42* as default value.

{{% notice info %}}
It's best to retrieve the service from the dependency resolver or let it be injected into the classes using it
{{% /notice %}}

## Setting values to the configuration

To store values in the configuration, use the following code:

```
var configurationService = new ConfigurationService();
configurationService.SetValue("mySetting", 42);
```

{{% notice info %}}
It's best to retrieve the service from the dependency resolver or let it be injected into the classes using it
{{% /notice %}}

## Customizing the way values are stored

The *ConfigurationService* is written with extensibility in mind. Though it defaults to the .NET local storage system, it is very easy to create a customized configuration service. Below is an example on how to customize the service so it reads and writes values from/to a database.

```
public class DbConfigurationService : ConfigurationService
{
    protected override bool ValueExists(string key)
    {
        using (var context = new ConfigurationContext())
        {
            return (from config in context.Configurations
                    where config.Key == key
                    select config).Any();
        }
    }
Â 
    protected override string GetValueFromStore(string key)
    {
        using (var context = new ConfigurationContext())
        {
            return (from config in context.Configurations
                    where config.Key == key
                    select config.Value).First();
        }
    }

    protected override void SetValueToStore(string key, string value)
    {
        using (var context = new ConfigurationContext())
        {
            var configuration (from config in context.Configurations
                    where config.Key == key
                    select config).FirstOrDefault();

            if (configuration == null)
            {
                configuration = context.CreateObject<Configuration>();
                configuration.Key = key;
            }

            configuration.Value = value;
            context.SaveChanges();
        }
    }
}
```

{{% notice warning %}}
Don't forget to register the customized *ConfigurationService* in the *ServiceLocator*
{{% /notice %}}

