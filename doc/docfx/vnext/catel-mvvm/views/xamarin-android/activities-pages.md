---
title: "Activities (pages)" 
---
The user controls in Android are called fragments. This means that if a user control must be created, it must derive from theÂ `Fragment`Â class. Catel provides a base implementation of this class to ensure full compatibility with the MVVM framework that ships with Catel.

## Creating the view model

The view model can be created (or added as a linked file from another project) just like any platform using Catel (they are all equal on all platforms).

## Creating the view

Make sure that aÂ `Views`Â folder exists in the project so the views and view models can automatically be hooked together by Catel. Then create a new class to the views folder, in this caseÂ `MainActivity`:

```
[Activity(MainLauncher = true)]
public class MainActivity : Catel.Android.App.Activity
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.Page_Main);
    }
}
```

Note that the class derives from *Catel.Android.App.Activity*

## Designing the view

To create the actual user interface of the fragment, add a newÂ `xaml`Â file to theÂ `Resources/layout`Â folder, in your caseÂ `Page\_Main`Â (but you can name it whatever you want). Then use the following source:

```
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:orientation="vertical"
    android:layout_width="fill_parent"
    android:layout_height="fill_parent">
    <Button
        android:id="@+id/MyButton"
        android:layout_width="fill_parent"
        android:layout_height="wrap_content"
        android:text="@string/Hello" />
</LinearLayout>
```

## Setting up synchronization

In Android it is required to manually synchronize the values between the view and view model. Below is the fully extended `MainActivity` class containing these mapping functionality:

```
[Activity(MainLauncher = true)]
public class MainActivity : Catel.Android.App.Activity
{
    private Button _testButton;

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.Page_Main);

        // Note: at this stage the visual tree is guaranteed in Android
        _testButton = FindViewById<Button>(Resource.Id.MyButton);
        _testButton.Click += delegate { GetViewModel<MainViewModel>().RunCommand.Execute(); };
    }

    protected override void SyncViewModel()
    {
        var vm = GetViewModel<MainViewModel>();
        if (vm == null)
        {
            return;
        }

        Title = vm.Title;
        _testButton.Text = string.Format("{0} clicks!", vm.Counter);
    }
}
```

