// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
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
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The type factory to use.
        /// </summary>
        private readonly ITypeFactory _typeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory" /> class.
        /// </summary>
        /// <param name="typeFactory">The type factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="typeFactory"/> is <c>null</c>.</exception>
        public ViewModelFactory(ITypeFactory typeFactory)
        {
            Argument.IsNotNull("typeFactory", typeFactory);

            _typeFactory = typeFactory;
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
        /// <returns>The newly created <see cref="IViewModel"/> or <c>null</c> if no view model could be created.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType"/> does not implement the <see cref="IViewModel"/> interface.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "CreateViewModel(Type, object, object)", TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public virtual IViewModel CreateViewModel(Type viewModelType, object dataContext)
        {
            return CreateViewModel(viewModelType, dataContext, null);
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
        public virtual IViewModel CreateViewModel(Type viewModelType, object dataContext, object tag)
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

        /// <summary>
        /// Creates a new view model.
        /// <para />
        /// This is a convenience wrapper around the <see cref="CreateViewModel(Type, object, object)"/> method. This method cannot be overriden.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <returns>The newly created <see cref="IViewModel"/> or <c>null</c> if no view model could be created.</returns>
        /// <exception cref="ArgumentException">The <c>TViewModel</c> does not implement the <see cref="IViewModel"/> interface.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "CreateViewModel<TViewModel>(Type, object, object)", TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public TViewModel CreateViewModel<TViewModel>(object dataContext)
            where TViewModel : IViewModel
        {
            var viewModelType = typeof(TViewModel);
            return (TViewModel)CreateViewModel(viewModelType, dataContext, null);
        }
    }
}