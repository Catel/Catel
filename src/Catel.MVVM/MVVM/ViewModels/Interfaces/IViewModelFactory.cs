namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Interface which allows custom instantation of view models. This way, if a view model contains a complex constructor or needs 
    /// caching, this factory can be used.
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        /// Determines whether the specified view model is a view model with model inject. A view model is
        /// considered a model injection if the first parameter of one of the constructors is not registered inside
        /// the dependency resolver.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>
        ///   <c>true</c> if the view model is a view model with model injection; otherwise, <c>false</c>.
        /// </returns>
        bool IsViewModelWithModelInjection(Type viewModelType);

        /// <summary>
        /// Determines whether the specified view model as data context can be reused and allow the view to set itself as
        /// owner of the inherited view model.
        /// <para />
        /// By default a view model is allowed to be inherited when it is of the same type as the expected view model type.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="expectedViewModelType">The expected view model type according to the view.</param>
        /// <param name="actualViewModelType">The actual view model type which is the type of the <paramref name="viewModelAsDataContext"/>.</param>
        /// <param name="viewModelAsDataContext">The view model as data context which must be checked.</param>
        /// <returns>
        ///   <c>true</c> if the specified view model instance ben be reused by the view; otherwise, <c>false</c>.
        /// </returns>
        bool CanReuseViewModel(Type viewType, Type expectedViewModelType, Type actualViewModelType, IViewModel? viewModelAsDataContext);

        /// <summary>
        /// Creates a new view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model that needs to be created.</param>
        /// <param name="dataContext">The data context of the view model.</param>
        /// <param name="tag">The preferred scope to use when resolving dependencies.</param>
        /// <returns>The newly created <see cref="IViewModel" /> or <c>null</c> if no view model could be created.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType" /> does not implement the <see cref="IViewModel" /> interface.</exception>
        IViewModel? CreateViewModel(Type viewModelType, object? dataContext, object? tag = null);
    }
}
