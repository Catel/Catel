namespace Catel.Services
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Controls;
    using Logging;
    using MVVM.Views;

    /// <summary>
    /// The view model wrapper service which is responsible of ensuring the view model container layer.
    /// </summary>
    public partial class ViewModelWrapperService : ViewModelWrapperServiceBase, IViewModelWrapperService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IViewModelWrapper _tempObject = new ViewModelWrapper(new Grid());
        private readonly ConditionalWeakTable<IView, IViewModelWrapper> _wrappers = new ConditionalWeakTable<IView, IViewModelWrapper>();

        /// <summary>
        /// Determines whether the specified view is already wrapped.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if the specified view is already wrapped; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public bool IsWrapped(IView view)
        {
            ArgumentNullException.ThrowIfNull(view);

            return IsViewWrapped(view);
        }

        /// <summary>
        /// Wraps the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModelSource">The view model source containing the <c>ViewModel</c> property.</param>
        /// <param name="wrapOptions">The wrap options.</param>
        /// <returns>The <see cref="IViewModelWrapper" />.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
        public IViewModelWrapper? Wrap(IView view, object viewModelSource, WrapOptions wrapOptions)
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(viewModelSource);

            if (!_wrappers.TryGetValue(view, out var wrapper))
            {
                try
                {
                    // This is "sort of" a lock, we might recursively call this while we are
                    // creating the wrapper, so need to skip *while* we are creating it
                    _wrappers.Add(view, _tempObject);

                    wrapper = CreateViewModelGrid(view, viewModelSource, wrapOptions);
                }
                finally
                {
                    // Remove the temp object
                    _wrappers.Remove(view);
                }

                if (wrapper is not null)
                {
                    _wrappers.Add(view, wrapper);
                }
            }

            return wrapper;
        }

        /// <summary>
        /// Gets the existing view model wrapper for the view. If there is none, this method will return <c>null</c>.
        /// </summary>
        /// <param name="view">The view to get the wrapper for.</param>
        /// <returns>The existing view model wrapper or <c>null</c> if there is no wrapper.</returns>
        public IViewModelWrapper? GetWrapper(IView view)
        {
            if (_wrappers.TryGetValue(view, out var wrapper))
            {
                return wrapper;
            }

            return null;
        }
    }
}
