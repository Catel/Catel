namespace Catel.Services
{
    using Catel.MVVM;

    public class UIVisualizerResult
    {
        public UIVisualizerResult(bool? result, UIVisualizerContext context, object window)
        {
            DialogResult = result;
            Context = context;
            Window = window;
        }

        public bool? DialogResult { get; }

        public UIVisualizerContext Context { get; }

        public object Window { get; }

        public TViewModel GetViewModel<TViewModel>()
            where TViewModel : class, IViewModel
        {
            return Context.Data as TViewModel;
        }
    }
}
