namespace Catel.MVVM.Views
{
    using System.Windows.Controls;

    public partial class ViewModelWrapper
    {
        private Grid _grid;

        partial void CreateWrapper(object viewModelWrapper)
        {
            _grid = (Grid)viewModelWrapper;
        }

        partial void SetViewModel(IViewModel viewModel)
        {
            _grid.DataContext = viewModel;
        }
    }
}
