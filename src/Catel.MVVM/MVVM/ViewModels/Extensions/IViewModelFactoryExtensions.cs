namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Extensions for the <see cref="IViewModelFactory"/>.
    /// </summary>
    public static class IViewModelFactoryExtensions
    {
        /// <summary>
        /// Creates a new view model.
        /// <para />
        /// This is a convenience wrapper around the <see cref="IViewModelFactory.CreateViewModel(Type, object, object)" /> method. This method cannot be overriden.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModelFactory">The view model factory.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="tag">The preferred tag to use when resolving dependencies.</param>
        /// <returns>The newly created <see cref="IViewModel" /> or <c>null</c> if no view model could be created.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelFactory" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <c>TViewModel</c> does not implement the <see cref="IViewModel" /> interface.</exception>
        public static TViewModel? CreateViewModel<TViewModel>(this IViewModelFactory viewModelFactory, object? dataContext, object? tag = null)
            where TViewModel : IViewModel
        {
            ArgumentNullException.ThrowIfNull(viewModelFactory);

            var viewModelType = typeof(TViewModel);
            return (TViewModel?)viewModelFactory.CreateViewModel(viewModelType, dataContext, tag);
        }

        /// <summary>
        /// Creates a new view model.
        /// <para />
        /// This is a convenience wrapper around the <see cref="IViewModelFactory.CreateViewModel(Type, object, object)" /> method. This method cannot be overriden.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory.</param>
        /// <param name="viewModelType">The type of the view model.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="tag">The preferred tag to use when resolving dependencies.</param>
        /// <returns>The newly created <see cref="IViewModel" /> or <c>null</c> if no view model could be created.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelFactory" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <c>TViewModel</c> does not implement the <see cref="IViewModel" /> interface.</exception>
        public static IViewModel CreateRequiredViewModel(this IViewModelFactory viewModelFactory, Type viewModelType, object? dataContext, object? tag = null)
        {
            ArgumentNullException.ThrowIfNull(viewModelFactory);

            var vm = viewModelFactory.CreateViewModel(viewModelType, dataContext, tag);
            if (vm is null)
            {
                throw new CatelException($"Cannot create view model '{viewModelType.FullName}' using data context '{ObjectToStringHelper.ToTypeString(dataContext)}'");
            }

            return vm;
        }

        /// <summary>
        /// Creates a new view model.
        /// <para />
        /// This is a convenience wrapper around the <see cref="IViewModelFactory.CreateViewModel(Type, object, object)" /> method. This method cannot be overriden.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModelFactory">The view model factory.</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="tag">The preferred tag to use when resolving dependencies.</param>
        /// <returns>The newly created <see cref="IViewModel" /> or <c>null</c> if no view model could be created.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelFactory" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <c>TViewModel</c> does not implement the <see cref="IViewModel" /> interface.</exception>
        public static TViewModel CreateRequiredViewModel<TViewModel>(this IViewModelFactory viewModelFactory, object? dataContext, object? tag = null)
        where TViewModel : IViewModel
        {
            ArgumentNullException.ThrowIfNull(viewModelFactory);

            var vm = viewModelFactory.CreateViewModel<TViewModel>(dataContext, tag);
            if (vm is null)
            {
                throw new CatelException($"Cannot create view model '{typeof(TViewModel).FullName}' using data context '{ObjectToStringHelper.ToTypeString(dataContext)}'");
            }

            return vm;
        }
    }
}
