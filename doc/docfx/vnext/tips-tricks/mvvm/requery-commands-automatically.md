---
title: "Requery commands automatically using CommandManager" 
---
For performance reasons, Catel no longer automatically subscribes to the *CommandManager* to invalidate the state (saves a *lot* of CanExecute calls). If you want this behavior back, you can create a custom class that subscribes to the command manager and invalidates the commands for you.

This was removed for a reason (performance), so this is not the recommended approach. But this allows you to get back the old behavior.

To use this class, instantiate it and register it in the *ServiceLocator* so it stays alive.

```
public class RequeryUsingCommandManager
{
    private IViewModelManager _viewModelManager;

    public RequeryUsingCommandManager(IViewModelManager viewModelManager)
    {
        Argument.IsNotNull(() => viewModelManager);

        _viewModelManager = viewModelManager;

        System.Windows.Input.CommandManager.RequerySuggested += OnCommandManagerRequerySuggested;
    }

    private void OnCommandManagerRequerySuggested(object sender, SomeEventArgs e)
    {
        InvalidateCommands();
    }

    private void InvalidateCommands()
    {
        var viewModels = _viewModelManager.ActiveViewModels;
        foreach (var viewModel in viewModels)
        {
            var viewModelBase = viewModel as ViewModelBase;
            if (viewModelBase != null)
            {
                var viewModelCommandManager = viewModelBase.GetViewModelCommandManager();
                viewModelCommandManager.InvalidateCommands();
            }
        }
    }
}
```

