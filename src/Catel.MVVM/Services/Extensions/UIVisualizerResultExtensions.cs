namespace Catel.Services;

using System;
using Catel.MVVM;

public static class UIVisualizerResultExtensions
{
    public static TViewModel? GetViewModel<TViewModel>(this UIVisualizerResult result)
        where TViewModel : class, IViewModel
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.Context.Data as TViewModel;
    }

    public static TViewModel GetRequiredViewModel<TViewModel>(this UIVisualizerResult result)
        where TViewModel : class, IViewModel
    {
        var vm = GetViewModel<TViewModel>(result);
        if (vm is null)
        {
            throw new InvalidOperationException($"The result does not contain a view model of type {typeof(TViewModel).FullName}");
        }

        return vm;
    }
}
