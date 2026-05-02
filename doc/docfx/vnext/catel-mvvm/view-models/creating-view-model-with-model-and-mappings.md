---
title: "Creating a view model with a model and mappings" 
---
During of the use of the MVVM pattern, we noticed that lots and lots of developers have a model, and map the values of the model to all properties of the view model. When the UI closes, the developers map all the properties back to the model. All this redundant code is not necessary when using the view models of Catel.

In Catel, we have created attributes that allow you to define a property as a model. A model is a property that a part of the view model represents to the user. A view model might have multiple models if it is a combination of several models.

To use the mapping features, the following attributes are very important:

-   `ModelAttribute` - declare a model in a view model
-   `ViewModelToModelAttribute` - declare a mapping from a view model property to a model property

In Catel 4.0 a new mapping mechanism is introduced that makes it possible to convert types of properties of the mappings between the `Model` and `ViewModel`. It is also possible to map to a collection of properties to a single property as result (like `MultiBinding` and `Converter` in WPF).

To use new mechanism you should declare this attributes:

-   ConverterType - declare a type of converter that converts properties, converter should implementÂ `IViewModelToModelConverter`. It is recommendedÂ to useÂ `ViewModelToModelConverterBase` as base class for custom converters. The default converter used isÂ `DefaultViewModelToModelMappingConverter` that provides basic 1:1 mappings between the model and view model.
-   AdditionalConstructorArgs -Â declare a arguments witch would be passed to converter constructor via reflection
-   AdditionalPropertiesToWatch -Â declare properties, which changes would trigger the converter

## Code snippets

-   vm - declare a view model
-   vmpropmodel - declare a property as model on a view model
-   vmpropviewmodeltomodel - declare a property as a pass-through property on a view model"

## Explanation

Defining a model is very simple, you only have to decorate your property with the `ModelAttribute`:

```
/// <summary>
/// Gets or sets the person.
/// </summary>
[Model]
public IPerson Person
{
    get { return GetValue<IPerson>(PersonProperty ); }
    private set { SetValue(PersonProperty , value); }
}

/// <summary>
/// Register the Person property so it is known in the class.
/// </summary>
public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(IPerson));
```

Using the `ModelAttribute` is very powerful. Basically, this is the extended functionality in the view model. If the model supports `IEditableObject`, `BeginEdit` is automatically called in the initialization of the view model. When the view model is canceled, the `CancelEdit` is called so the changes are undone.

When a model is defined, it is possible to use the `ViewModelToModelAttribute`, as you can see in the code below:

```
/// <summary>
/// Gets or sets the FirstName of the person.
/// </summary>
[ViewModelToModel("Person")]
public string FirstName
{
    get { return GetValue<string>(FirstNameProperty); }
    set { SetValue(FirstNameProperty, value); }
}

/// <summary>
/// Register the FirstName property so it is known in the class.
/// </summary>
public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string));
Â 
/// <summary>
/// Gets or sets the LastName of the person.
/// </summary>
[ViewModelToModel("Person")]
public string LastName
{
    get { return GetValue<string>(LastNameProperty); }
    set { SetValue(LastNameProperty, value); }
}

/// <summary>
/// Register the LastName property so it is known in the class.
/// </summary>
public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string));
```

If there is a single model on a view model, the name of the model in the `ViewModelToModel` can be ommitted as shown in the code below:

```
[ViewModelToModel]
public string FirstName
{
    get { return GetValue<string>(FirstNameProperty); }
    set { SetValue(FirstNameProperty, value); }
}

public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string));
```

The `ViewModelToModelAttribute` in the code example above automatically maps the view model `FirstName`Â and `LastName` properties to the `Person.FirstName` andÂ `Person.LastName`Â properties. This way, you donâ€™t have to manually map the values from and to the model. Another nice effect is that the view model automatically validates all objects defined using the `ModelAttribute`, and all field and business errors mapped are automatically mapped to the view model.

Sometimes you need the full name of a person, you can easily acquire it by creating a custom converter:

```
      public class CollapsMappingConverter : DefaultViewModelToModelMappingConverter
    {
        #region Fields
        private readonly char _separator;
        #endregion

        #region Constructors
        public CollapsMappingConverter(string[] propertyNames)
            : this(propertyNames, ' ')
        { }

        public CollapsMappingConverter(string[] propertyNames, char separator = ' ')
            : base(propertyNames)
        {
            _separator = separator;
        }
        #endregion

        #region Properties
        public char Separator
        {
            get { return _separator; }
        }
        #endregion

        #region Methods
        public override bool CanConvert(Type[] types, Type outType, Type viewModelType)
        {
            return types.All(x => x == typeof (string)) && outType == typeof (string); //check that all input and output values are strings
        }

        public override object Convert(object[] values, IViewModel viewModel)
        {
            return string.Join(Separator.ToString(), values.Where(x => !string.IsNullOrWhiteSpace((string) x)));
        }

        public override bool CanConvertBack(Type inType, Type[] outTypes, Type viewModelType)
        {
            return outTypes.All(x => x == typeof (string)) && inType == typeof (string); //check that all input and output values are strings
        }

        public override object[] ConvertBack(object value, IViewModel viewModel)
        {
            return ((string) value).Split(Separator);
        }
        #endregion
    }
```

Now, when we created the converter we should define it in mapping like this:

```
/// <summary>
/// Gets or sets the FullName of the person.
/// </summary>
[ViewModelToModel("Person", "FirstName", AdditionalPropertiesToWatch = new[] { "LastName" }, ConverterType = typeof(CollapsMappingConverter))]
public string FullName
{
    get { return GetValue<string>(FullNameProperty); }
    set { SetValue(FullNameProperty, value); }
}

/// <summary>
/// Register the LastName property so it is known in the class.
/// </summary>
public static readonly PropertyData FullNameProperty = RegisterProperty("FullName", typeof(string));
```

TheÂ `ViewModelToModelAttribute`Â in the code example above automatically maps the view modelÂ `FullName` property to theÂ `Person.FirstName`Â andÂ `Person.LastName`Â properties and converts them with `CollapsMappingConverter`. This way, you donâ€™t have to manually map the values from the model and update `FullName` property when `FirstName`Â or `LastName` property changed.

Summarized, the `Model` and `ViewModelToModel` attributes make sure no duplicate validation and no manual mappings are required.

Â 

