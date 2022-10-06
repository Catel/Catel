namespace Catel.Services
{
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
    }
}
