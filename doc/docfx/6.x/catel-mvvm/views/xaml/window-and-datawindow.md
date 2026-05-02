---
title: "Window and DataWindow" 
---
## Introduction to the Window

The Window is a simplified class representing the same logic as the regular System.Windows.Window but with the binding support of Catel.

## Introduction to the DataWindow

When developing software in XAML, most developers always need the following three types of windows:

-   OK / Cancel buttons for data windows;
-   OK / Cancel / Apply buttons for application settings / options;
-   Close button on windows for action windows.

Creating these windows is just boring and the steps are always the same:

The `DataWindow` class makes it much easier to create these basic windows, simply by specifying the mode of the *Window*. By using this window, you can concentrate on the actual implementation and you donâ€™t have to worry about the implementation of the buttons itself, which saves you time!Â 

## Using the DataWindow in MVVM

The easiest object to use with the MVVM Framework is the `DataWindow` class. The `DataWindow` takes fully care of the construction of the view models and the validation of the view models.

The usage of the `DataWindow` class is very simple, once you know how to do it. First of all, you will have to specify the base class in the xaml file like shown below:

```
<catel:DataWindow x:Class="Catel.Articles._03___MVVM.Examples.DataWindow.PersonWindow"
                  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                  xmlns:catel="http://schemas.catelproject.com">

    <!-- Content left out for the sake of simplicity -->

</catel:DataWindow>
```

As you can see, one thing has changed in regard to a â€œnormalâ€ window definition:

1.  The type definition has changed from `Window` to `catel:DataWindow`;

The code-behind is even simpler:

```
/// <summary>
/// Interaction logic for PersonWindow.xaml
/// </summary>
public partial class PersonWindow : DataWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonWindow"/> class.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    public PersonWindow(PersonViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
```

The code above is everything you will need when using the MVVM Framework of Catel.Â 

The easiest way to create a new `DataWindow` is to use item templates

## Construction of view models

There are multiple ways to construct a window with a view model. There are three options that you have to construct a view model:

-   **Constructor with view model**
    This is the best option you can use. This way, it is possible to inject view models into the data window.
-   **Constructor with model**
    It is possible to save a developer from creating a view model manually by accepting a model as input. Then, the data window will have to construct the view model manually and pass it through to its base constructor.
-   **Empty constructor**
    If you use an empty constructor, the developer will have to set the data context manually. This something you really should avoid. But hey, itâ€™s all up to you.

## Automatic validation

The cool thing about the `DataWindow` is that it automatically wraps the content that a developer defines into an `InfoBarMessageControl`. This way, errors and warnings are shown at the top of the window. Another feature of the `DataWindow` is that it automatically creates a `WarningAndErrorValidator` control and sets the view model as source. This way, all the warnings of the view model are also shown in the `InfoBarMessageControl`. In other words: you donâ€™t have to do anything to implementation validation, except for actually setting the warnings and errors in your view model. And if the validation takes place in the model, you can use the `ViewModelToModelAttribute` so you donâ€™t have to worry about that either.

## Customizing the buttons

It is possible to use custom buttons and still be able to use the `DataWindow`.

1. First, use the base constructor to specify that you want to use custom mode.

```
/// <summary>
/// Upload window.
/// </summary>
public class UploadWindow : DataWindow
{
    public UploadWindow()
        : base(DataWindowMode.Custom)
    {
        InitializeComponent();
    }
}
```

2. Add the custom buttons. This must be done before the call to `InitializeComponent`.

```
/// <summary>
/// Upload window.
/// </summary>
public class UploadWindow : DataWindow
{
    public UploadWindow()
        : base(DataWindowMode.Custom)
    {
        AddCustomButton(new DataWindowButton("Upload", "Upload"));

        InitializeComponent();
    }
}
```

## Styling the DataWindow

Starting with Catel 2.4, the `DataWindow` has its own styles. These are located in `DataWindow.generic.xaml`. Below is a table that contains the available styles and a short description.

Style key|Description
---|---
DataWindowStyle|The actual window style which can be used to decorate or customize the window itself.
DataWindowButtonContainerStyle|The container that is used for the buttons. This is a `WrapPanel`, so the styles must match that.
DataWindowButtonStyle|The style for the buttons. By default, the buttons are right aligned and have a fixed size.

## Closing the DataWindow with the Escape key

The `DataWindow` can be configured to close with the Escape key using the `CanCloseUsingEscape` dependency property (default `true`). This is useful for when `DataWindow` is used in e.g. dialogs. When used as the main window, it is advised to set `CanCloseUsingEscape=False`, as the user can inadvertedly close the application by pressing the `Escape` key.

