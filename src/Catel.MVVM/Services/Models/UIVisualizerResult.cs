namespace Catel.Services
{
    using Catel.MVVM;

    public class UIVisualizerResult
    {
        public UIVisualizerResult(bool? result, object data, object dataContext, object window)
        {
            DialogResult = result;
            Data = data;
            DataContext = dataContext ?? data;
            Window = window;
        }

        public bool? DialogResult { get; }

        public object Data { get; }

        public object DataContext { get; }

        public object Window { get; }

        public TViewModel GetViewModel<TViewModel>()
            where TViewModel : class, IViewModel
        {
            return DataContext as TViewModel;
        }
    }
}
