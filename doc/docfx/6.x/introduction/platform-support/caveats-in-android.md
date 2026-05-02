---
title: "Caveats in Android" 
---
Below are all caveats in Android.

-   [Linker settings](#linker-settings)

Know caveats? Feel free to add them!

## Linker settings

When linking in release mode (or debug if you would like), the linker will remove all non-used items from the final application assembly. Since the binding system in Catel uses reflection, it might break when the linker is too aggressive when optimizing the app. To prevent optimalization, create a dummy file that uses the members of each type so the linker will not exclude them. Note that this class will never be instantiated, nor will its methods be invoked. It is purely to let the static analysis be notified of the usage.

Note that this applies to both properties and bindings

Below is an example class to force the inclusion of members in Android. For each type and its members, a method is added. Then each used property is accessed and each used event is subscribed to.

```
public class LinkerInclude
{
    public void IncludeActivity(Activity activity)
    {
        activity.Title = activity.Title = string.Empty;
    }

    public void IncludeButton(Button button)
    {
        button.Text = button.Text + string.Empty;

        button.Click += (sender, e) => {  };
    }

    public void IncludeEditText(EditText editText)
    {
        editText.Text = editText.Text + string.Empty;

        editText.TextChanged += (sender, e) => { };
    }

    public void IncludeCommand(ICatelCommand command)
    {
        command.CanExecuteChanged += (sender, e) => { };
    }
}
```

