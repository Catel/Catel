namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// View model wrapper class.
    /// </summary>
    public partial class ViewModelWrapper : IViewModelWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelWrapper" /> class.
        /// </summary>
        /// <param name="contentToWrap">The view model wrapper object, such as a grid.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="contentToWrap" /> is <c>null</c>.</exception>
        public ViewModelWrapper(object contentToWrap)
        {
            ArgumentNullException.ThrowIfNull(contentToWrap);

            CreateWrapper(contentToWrap);
        }

        /// <summary>
        /// Updates the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void UpdateViewModel(IViewModel viewModel)
        {
            SetViewModel(viewModel);
        }

        partial void CreateWrapper(object viewModelWrapper);
        partial void SetViewModel(IViewModel viewModel);
    }
}
