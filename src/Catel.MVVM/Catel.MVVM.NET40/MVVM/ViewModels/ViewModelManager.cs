// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;
    using Reflection;

    /// <summary>
    /// Manager for view models. Thanks to this manager, it is possible to subscribe to other view models and be able to respond
    /// correctly to property changes in other views.
    /// </summary>
    public class ViewModelManager : IViewModelManager
    {
        #region Fields
        /// <summary>
        /// The <see cref="ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// List of all live instances of the view model managers.
        /// </summary>
        private static readonly List<ViewModelManager> _instances = new List<ViewModelManager>();

        /// <summary>
        /// The lock for the _managedViewModels dictionary.
        /// </summary>
        private readonly object _managedViewModelsLock = new object();

        /// <summary>
        /// Dictionary containing all the managed view models by this view model manager.
        /// </summary>
        private readonly Dictionary<Type, ManagedViewModel> _managedViewModels = new Dictionary<Type, ManagedViewModel>();

        /// <summary>
        /// The lock for the _viewModelModels dictionary.
        /// </summary>
        private readonly object _viewModelModelsLock = new object();

        /// <summary>
        /// Dictionary containing the unique identifiers of a all view models and their registered models.
        /// </summary>
        private readonly Dictionary<int, List<object>> _viewModelModels = new Dictionary<int, List<object>>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelManager"/> class.
        /// </summary>
        /// <remarks>
        /// The constructor is private because this is a singleton class.
        /// </remarks>
        public ViewModelManager()
        {
            _instances.Add(this);

            Log.Debug("ViewModelManager instantiated");
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the view model count.
        /// </summary>
        /// <value>The view model count.</value>
        public int ViewModelCount
        {
            get
            {
                int count = 0;

                lock (_managedViewModelsLock)
                {
                    foreach (var managedViewModel in _managedViewModels)
                    {
                        if (managedViewModel.Value != null)
                        {
                            count += managedViewModel.Value.ViewModelCount;
                        }
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the active view models presently registered.
        /// </summary>
        public IEnumerable<IViewModel> ActiveViewModels
        {
            get
            {
                lock (_managedViewModelsLock)
                {
                    return _managedViewModels.SelectMany(row => row.Value.ViewModels);
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Registers the model of a view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public void RegisterModel(IViewModel viewModel, object model)
        {
            Argument.IsNotNull("viewModel", viewModel);
            Argument.IsNotNull("model", model);

            var viewModelTypeName = ObjectToStringHelper.ToTypeString(viewModel);
            var modelTypeName = ObjectToStringHelper.ToTypeString(model);

            Log.Debug("Registering model '{0}' with view model '{1}' (id = '{2}')", modelTypeName, viewModelTypeName, viewModel.UniqueIdentifier);

            lock (_viewModelModelsLock)
            {
                if (!_viewModelModels.ContainsKey(viewModel.UniqueIdentifier))
                {
                    _viewModelModels[viewModel.UniqueIdentifier] = new List<object>();
                }

                var models = _viewModelModels[viewModel.UniqueIdentifier];
                models.Add(model);
            }

            Log.Debug("Registered model '{0}' with view model '{1}' (id = '{2}')", modelTypeName, viewModelTypeName, viewModel.UniqueIdentifier);
        }

        /// <summary>
        /// Unregisters the model of a view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public void UnregisterModel(IViewModel viewModel, object model)
        {
            Argument.IsNotNull("viewModel", viewModel);
            Argument.IsNotNull("model", model);

            var viewModelTypeName = ObjectToStringHelper.ToTypeString(viewModel);
            var modelTypeName = ObjectToStringHelper.ToTypeString(model);

            Log.Debug("Unregistering model '{0}' with view model '{1}' (id = '{2}')", modelTypeName, viewModelTypeName, viewModel.UniqueIdentifier);

            lock (_viewModelModelsLock)
            {
                if (!_viewModelModels.ContainsKey(viewModel.UniqueIdentifier))
                {
                    _viewModelModels[viewModel.UniqueIdentifier] = new List<object>();
                }

                var models = _viewModelModels[viewModel.UniqueIdentifier];
                models.Remove(model);
            }

            Log.Debug("Unregistered model '{0}' with view model '{1}' (id = '{2}')", modelTypeName, viewModelTypeName, viewModel.UniqueIdentifier);
        }

        /// <summary>
        /// Unregisters all models of a view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public void UnregisterAllModels(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var viewModelTypeName = ObjectToStringHelper.ToTypeString(viewModel);
            int modelCount = 0;

            Log.Debug("Unregistering all models of view model '{0}' (id = '{1}')", viewModelTypeName, viewModel.UniqueIdentifier);

            lock (_viewModelModelsLock)
            {
                if (!_viewModelModels.ContainsKey(viewModel.UniqueIdentifier))
                {
                    _viewModelModels[viewModel.UniqueIdentifier] = new List<object>();
                }

                var models = _viewModelModels[viewModel.UniqueIdentifier];
                modelCount = models.Count;
                models.Clear();
            }

            Log.Debug("Unregistered all '{0}' models of view model '{1}' (id = '{2}')", modelCount, viewModelTypeName, viewModel.UniqueIdentifier);
        }

        /// <summary>
        /// Gets the view models of a model.
        /// </summary>
        /// <param name="model">The model to find the linked view models for.</param>
        /// <returns>An array containing all the view models.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public IViewModel[] GetViewModelsOfModel(object model)
        {
            Argument.IsNotNull("model", model);

            var modelType = ObjectToStringHelper.ToTypeString(model);

            Log.Debug("Getting all view models that are linked to model '{0}'", modelType);

            var viewModels = new List<IViewModel>();

            lock (_viewModelModelsLock)
            {
                foreach (var viewModelModel in _viewModelModels)
                {
                    var viewModelIdentifiers = (from m in viewModelModel.Value
                                                where ObjectHelper.AreEqualReferences(m, model)
                                                select viewModelModel.Key);

                    foreach (var viewModelIdentifier in viewModelIdentifiers)
                    {
                        var vm = GetViewModel(viewModelIdentifier);
                        if (vm != null)
                        {
                            viewModels.Add(vm);
                        }
                    }
                }
            }

            Log.Debug("Found '{0}' view models that are linked to model '{1}'", viewModels.Count, modelType);

            return viewModels.ToArray();
        }

        /// <summary>
        /// Gets the view model by its unique identifier.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <returns>The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.</returns>
        public IViewModel GetViewModel(int uniqueIdentifier)
        {
            Log.Debug("Searching for the instance of view model with unique identifier '{0}'", uniqueIdentifier);

            lock (_managedViewModelsLock)
            {
                foreach (var managedViewModel in _managedViewModels)
                {
                    foreach (var viewModel in managedViewModel.Value.ViewModels)
                    {
                        if (viewModel.UniqueIdentifier == uniqueIdentifier)
                        {
                            Log.Debug("Found the instance of view model with unique identifier '{0}' as type '{1}'", uniqueIdentifier, ObjectToStringHelper.ToTypeString(viewModel));

                            return viewModel;
                        }
                    }
                }
            }

            Log.Debug("Did not find the instance of view model with unique identifier '{0}'. It is either not registered or not alive.", uniqueIdentifier);

            return null;
        }

        /// <summary>
        /// Gets the first or default instance of the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>
        /// The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.
        /// </returns>
        public TViewModel GetFirstOrDefaultInstance<TViewModel>() where TViewModel : IViewModel
        {
            var viewModelType = typeof(TViewModel);

            return (TViewModel)GetFirstOrDefaultInstance(viewModelType);
        }

        /// <summary>
        /// Gets the first or default instance of the specified view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view mode.</param>
        /// <returns>
        /// The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.
        /// </returns>
        /// <exception cref="System.ArgumentException">The <paramref name="viewModelType"/> is not of type <see cref="IViewModel"/>.</exception>
        public IViewModel GetFirstOrDefaultInstance(Type viewModelType)
        {
            Argument.IsOfType("viewModelType", viewModelType, typeof (IViewModel));

            return ActiveViewModels.FirstOrDefault(viewModel => ObjectHelper.AreEqual(viewModel.GetType(), viewModelType));
        }

        /// <summary>
        /// Gets the child view models of the specified view model.
        /// </summary>
        /// <param name="parentViewModel">The parent view model.</param>
        /// <returns>The child view models.</returns>
        public IEnumerable<IRelationalViewModel> GetChildViewModels(IViewModel parentViewModel)
        {
            Argument.IsNotNull(() => parentViewModel);

            var childViewModels = GetChildViewModels(parentViewModel.UniqueIdentifier);

            return childViewModels;
        }

        /// <summary>
        /// Gets the child view models of the specified view model unique identifier.
        /// </summary>
        /// <param name="parentUniqueIdentifier">The parent unique identifier.</param>
        /// <returns>The child view models.</returns>
        public IEnumerable<IRelationalViewModel> GetChildViewModels(int parentUniqueIdentifier)
        {
            var relationalViewModels = ActiveViewModels.OfType<IRelationalViewModel>();

            var childViewModels = relationalViewModels.Where(viewModel => viewModel.ParentViewModel != null && viewModel.ParentViewModel.UniqueIdentifier == parentUniqueIdentifier);

            return childViewModels;
        }

        /// <summary>
        /// Clears all the current view model managers.
        /// </summary>
        /// <remarks>
        /// This method should only be called during unit testing.
        /// </remarks>
        internal static void ClearAll()
        {
            foreach (var manager in _instances)
            {
                manager.Clear();
            }
        }

        /// <summary>
        /// Clears all the view models in the manager.
        /// </summary>
        /// <remarks>
        /// This method should only be called during unit testing.
        /// </remarks>
        internal void Clear()
        {
            lock (_managedViewModelsLock)
            {
                foreach (var viewModel in _managedViewModels)
                {
                    viewModel.Value.Clear();
                }

                _managedViewModels.Clear();
            }
        }

        /// <summary>
        /// Registers a view model instance with the manager. All view models must register themselves to the manager.
        /// </summary>
        /// <param name="viewModel">The view model to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        internal void RegisterViewModelInstance(IViewModel viewModel)
        {
            RegisterViewModelInstanceInternal(viewModel);
        }

        /// <summary>
        /// Registers a view model instance with the manager. All view models must register themselves to the manager.
        /// </summary>
        /// <param name="viewModel">The view model to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        private void RegisterViewModelInstanceInternal(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var managedViewModel = GetManagedViewModel(viewModel.GetType());
            managedViewModel.AddViewModelInstance(viewModel);
        }

        /// <summary>
        /// Unregisters a view model instance from the manager. All view models must unregister themselves from the manager.
        /// </summary>
        /// <param name="viewModel">The view model to unregister.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        internal void UnregisterViewModelInstance(IViewModel viewModel)
        {
            UnregisterViewModelInstanceInternal(viewModel);
        }

        /// <summary>
        /// Unregisters a view model instance from the manager. All view models must unregister themselves from the manager.
        /// </summary>
        /// <param name="viewModel">The view model to unregister.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        private void UnregisterViewModelInstanceInternal(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var managedViewModel = GetManagedViewModel(viewModel.GetType());
            managedViewModel.RemoveViewModelInstance(viewModel);
        }

        /// <summary>
        /// Adds an interested view model instance. The <see cref="IViewModel"/> class will automatically register
        /// itself to the manager by using this method when decorated with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model the <paramref name="viewModel"/> is interested in.</param>
        /// <param name="viewModel">The view model instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        internal void AddInterestedViewModelInstance(Type viewModelType, IViewModel viewModel)
        {
            AddInterestedViewModelInstanceInternal(viewModelType, viewModel);
        }

        /// <summary>
        /// Adds an interested view model instance. The <see cref="IViewModel"/> class will automatically register
        /// itself to the manager by using this method when decorated with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model the <paramref name="viewModel"/> is interested in.</param>
        /// <param name="viewModel">The view model instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        private void AddInterestedViewModelInstanceInternal(Type viewModelType, IViewModel viewModel)
        {
            Argument.IsNotNull("viewModelType", viewModelType);
            Argument.IsNotNull("viewModel", viewModel);

            var managedViewModel = GetManagedViewModel(viewModelType);
            managedViewModel.AddInterestedViewModel(viewModel);
        }

        /// <summary>
        /// Removes an interested view model instance. The <see cref="IViewModel"/> class will automatically unregister
        /// itself from the manager by using this method when decorated with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model the <paramref name="viewModel"/> was interested in.</param>
        /// <param name="viewModel">The view model instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        internal void RemoveInterestedViewModelInstance(Type viewModelType, IViewModel viewModel)
        {
            RemoveInterestedViewModelInstanceInternal(viewModelType, viewModel);
        }

        /// <summary>
        /// Removes an interested view model instance. The <see cref="IViewModel"/> class will automatically unregister
        /// itself from the manager by using this method when decorated with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model the <paramref name="viewModel"/> was interested in.</param>
        /// <param name="viewModel">The view model instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        private void RemoveInterestedViewModelInstanceInternal(Type viewModelType, IViewModel viewModel)
        {
            Argument.IsNotNull("viewModelType", viewModelType);
            Argument.IsNotNull("viewModel", viewModel);

            var managedViewModel = GetManagedViewModel(viewModelType);
            managedViewModel.RemoveInterestedViewModel(viewModel);
        }

        /// <summary>
        /// Gets the managed view model for a specific view model type.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>The <see cref="ManagedViewModel"/> of the specified type.</returns>
        private ManagedViewModel GetManagedViewModel(Type viewModelType)
        {
            lock (_managedViewModelsLock)
            {
                if (!_managedViewModels.ContainsKey(viewModelType))
                {
                    _managedViewModels.Add(viewModelType, new ManagedViewModel(viewModelType));
                }

                return _managedViewModels[viewModelType];
            }
        }
        #endregion
    }
}
