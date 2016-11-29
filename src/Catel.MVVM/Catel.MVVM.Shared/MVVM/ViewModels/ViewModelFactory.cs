// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Linq;
    using Caching;
    using IoC;
    using Logging;
    using Reflection;

    /// <summary>
    /// Default implementation of the <see cref="IViewModelFactory"/> which allows custom instantation of view models. This way,
    /// if a view model contains a complex constructor or needs caching, this factory can be used.
    /// <para />
    /// This default implementation will first try to inject the data context into the view model constructor. If that is not possible,
    /// it will try to call the empty or default constructor.
    /// </summary>
    public class ViewModelFactory : IViewModelFactory
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ITypeFactory _typeFactory;
        private readonly IDependencyResolver _dependencyResolver;

        private readonly ICacheStorage<Type, bool> _viewModelInjectionCache = new CacheStorage<Type, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory" /> class.
        /// </summary>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory" /> is <c>null</c>.</exception>
        public ViewModelFactory(ITypeFactory typeFactory, IDependencyResolver dependencyResolver)
        {
            Argument.IsNotNull("typeFactory", typeFactory);
            Argument.IsNotNull("dependencyResolver", dependencyResolver);

            _typeFactory = typeFactory;
            _dependencyResolver = dependencyResolver;
        }

        /// <summary>
        /// Determines whether the specified view model is a view model with model inject. A view model is
        /// considered a model injection if the first parameter of one of the constructors is not registered inside
        /// the dependency resolver.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>
        ///   <c>true</c> if the view model is a view model with model injection; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsViewModelWithModelInjection(Type viewModelType)
        {
            var isViewModelWithModelInjection = _viewModelInjectionCache.GetFromCacheOrFetch(viewModelType, () =>
            {
                var constructors = viewModelType.GetConstructorsEx();

                foreach (var constructor in constructors)
                {
                    var firstParameter = constructor.GetParameters().FirstOrDefault();
                    if (firstParameter != null)
                    {
                        if (!_dependencyResolver.CanResolve(firstParameter.ParameterType))
                        {
                            return true;
                        }
                    }
                }

                return false;
            });

            return isViewModelWithModelInjection;
        }

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
        public virtual bool CanReuseViewModel(Type viewType, Type expectedViewModelType, Type actualViewModelType, IViewModel viewModelAsDataContext)
        {
            if (viewModelAsDataContext == null)
            {
                return false;
            }

            return expectedViewModelType.IsInstanceOfTypeEx(viewModelAsDataContext);
        }

        /// <summary>
        /// Creates a new view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model that needs to be created.</param>
        /// <param name="dataContext">The data context of the view model.</param>
        /// <param name="tag">The preferred tag to use when resolving dependencies.</param>
        /// <returns>The newly created <see cref="IViewModel"/> or <c>null</c> if no view model could be created.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType"/> does not implement the <see cref="IViewModel"/> interface.</exception>
        public virtual IViewModel CreateViewModel(Type viewModelType, object dataContext, object tag = null)
        {
            Argument.IsNotNull("viewModelType", viewModelType);
            Argument.ImplementsInterface("viewModelType", viewModelType, typeof(IViewModel));

            IViewModel viewModel = null;

            // Only try to construct the view model when the injection object is not null, otherwise every
            // view model can be constructed with a nullable object. If a user wants a view model to be constructed
            // without any datacontext or injection, he/she should use an empty default constructor which will only
            // be used when injection is not possible
            if (dataContext != null)
            {
                viewModel = _typeFactory.CreateInstanceWithParametersAndAutoCompletionWithTag(viewModelType, tag, dataContext) as IViewModel;
                if (viewModel != null)
                {
                    Log.Debug("Constructed view model '{0}' using injection of data context '{1}'", viewModelType.FullName, ObjectToStringHelper.ToTypeString(dataContext));
                    return viewModel;
                }
            }

            // Try to construct view model using dependency injection
            viewModel = _typeFactory.CreateInstanceWithTag(viewModelType, tag) as IViewModel;
            if (viewModel != null)
            {
                Log.Debug("Constructed view model '{0}' using dependency injection or empty constructor", viewModelType.FullName);
                return viewModel;
            }

            Log.Debug("Could not construct view model '{0}' using injection of data context '{1}'",
                viewModelType.FullName, ObjectToStringHelper.ToTypeString(dataContext));

            return viewModel;
        }
    }
}