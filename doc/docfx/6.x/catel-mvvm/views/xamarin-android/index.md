---
title: "Xamarin.Android" 
---
Note that this guide is not a full Android development guide. It will cover the basics though.

## Bindings

Unfortunately Android does not have a powerful binding system like XAML does. Therefore it is required to manually synchronize data from the view to the view model and back or to use the binding system in Catel.

### Binding properties

```
[Activity]
public class SecondActivity : Catel.Android.App.Activity
{
    private PersonView _personView;

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.Page_Second);

        _personView = FragmentManager.FindFragmentById<PersonView>(Resource.Id.PersonView);
    }
    protected override void AddBindings(BindingContext bindingContext, IViewModel viewModel)
    {
        var vm = (SecondViewModel) viewModel;

        bindingContext.AddBinding(() => vm.Title, () => Title);
        bindingContext.AddBinding(() => vm.Person, () => _personView.DataContext);
    }
}
```

### Binding properties with events

```
public class PersonView : Catel.Android.App.Fragment
{
    private EditText _firstNameEditText;
    private EditText _lastNameEditText;
    private TextView _lastNameMirrorTextView ;

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var inflateResult = inflater.Inflate(Resource.Layout.Fragment_Person, container, false);
        return inflateResult;
    }

    public override void OnResume()
    {
        _firstNameEditText = Activity.FindViewById<EditText>(Resource.Id.firstNameText);
        _lastNameEditText = Activity.FindViewById<EditText>(Resource.Id.lastNameText);
        _lastNameMirrorTextView = Activity.FindViewById<TextView>(Resource.Id.lastNameMirrorTextView);

        base.OnResume();
    }

    protected override void AddBindings(BindingContext bindingContext, IViewModel viewModel)
    {
        var vm = (PersonViewModel)viewModel;

        bindingContext.AddBinding(() => vm.FirstName, () => _firstNameEditText.Text).AddTargetEvent("TextChanged");
        bindingContext.AddBinding(() => vm.LastName, () => _lastNameEditText.Text).AddTargetEvent("TextChanged");
        bindingContext.AddBinding(() => vm.LastName, () => _lastNameMirrorTextView.Text, BindingMode.OneWay);
    }
}
```

### Binding properties with converters

```
protected override void AddBindings(BindingContext bindingContext, IViewModel viewModel)
{
    var vm = (MainViewModel)viewModel;

    bindingContext.AddBindingWithConverter<ClicksConverter>(() => vm.Counter, () => _testButton.Text, BindingMode.OneWay);
}
```

### Binding commands

Â 

```
protected override void AddBindings(BindingContext bindingContext, IViewModel viewModel)
{
    var vm = (MainViewModel)viewModel;

    bindingContext.AddCommandBinding(_testButton, "Click", vm.RunCommand);
}
```

