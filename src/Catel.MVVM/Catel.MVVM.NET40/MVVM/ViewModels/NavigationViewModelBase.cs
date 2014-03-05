// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationViewModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System.ComponentModel;

    using Catel.Services;

    using IoC;

    /// <summary>
    /// Extended base class for view models that include navigation.
    /// <para />
    /// This class adds navigation commands and the navigation service.
    /// </summary>
    public abstract class NavigationViewModelBase : ViewModelBase
    {
        #region Fields
        private readonly INavigationService _navigationService;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <param name="supportIEditableObject">if set to <c>true</c>, the view model will natively support models that
        /// implement the <see cref="IEditableObject"/> interface.</param>
        /// <param name="ignoreMultipleModelsWarning">if set to <c>true</c>, the warning when using multiple models is ignored.</param>
        /// <param name="skipViewModelAttributesInitialization">
        /// if set to <c>true</c>, the initialization will be skipped and must be done manually via <see cref="ViewModelBase.InitializeViewModelAttributes"/>.
        /// </param>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
        protected NavigationViewModelBase(bool supportIEditableObject = true, bool ignoreMultipleModelsWarning = false, bool skipViewModelAttributesInitialization = false)
            : this(null, supportIEditableObject, ignoreMultipleModelsWarning, skipViewModelAttributesInitialization)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// <para/>
        /// This constructor allows the injection of a custom <see cref="IServiceLocator"/>.
        /// </summary>
        /// <param name="serviceLocator">The service locator to inject. If <c>null</c>, the <see cref="Catel.IoC.ServiceLocator.Default"/> will be used.</param>
        /// <param name="supportIEditableObject">if set to <c>true</c>, the view model will natively support models that
        /// implement the <see cref="IEditableObject"/> interface.</param>
        /// <param name="ignoreMultipleModelsWarning">if set to <c>true</c>, the warning when using multiple models is ignored.</param>
        /// <param name="skipViewModelAttributesInitialization">if set to <c>true</c>, the initialization will be skipped and must be done manually via <see cref="ViewModelBase.InitializeViewModelAttributes"/>.</param>
        /// <exception cref="ModelNotRegisteredException">A mapped model is not registered.</exception>
        /// <exception cref="PropertyNotFoundInModelException">A mapped model property is not found.</exception>
        protected NavigationViewModelBase(IServiceLocator serviceLocator, bool supportIEditableObject = true, bool ignoreMultipleModelsWarning = false,
                                          bool skipViewModelAttributesInitialization = false)
            : base(serviceLocator, supportIEditableObject, ignoreMultipleModelsWarning, skipViewModelAttributesInitialization)
        {
            Back = new Command(OnBackExecute, OnBackCanExecute);
            Forward = new Command(OnForwardExecute, OnForwardCanExecute);

            _navigationService = DependencyResolver.Resolve<INavigationService>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the navigation service which can be used to navigate inside an application.
        /// </summary>
        /// <value>The navigation service.</value>
        public INavigationService NavigationService
        {
            get { return _navigationService; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Back command.
        /// </summary>
        public Command Back { get; private set; }

        /// <summary>
        /// Gets the Forward command.
        /// </summary>
        public Command Forward { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Method to check whether the Back command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        protected virtual bool OnBackCanExecute()
        {
            return _navigationService.CanGoBack;
        }

        /// <summary>
        /// Method to invoke when the Back command is executed.
        /// </summary>
        private void OnBackExecute()
        {
            if (!_navigationService.CanGoBack)
            {
                return;
            }

            _navigationService.GoBack();
        }

        /// <summary>
        /// Method to check whether the Forward command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        protected virtual bool OnForwardCanExecute()
        {
            return _navigationService.CanGoForward;
        }

        /// <summary>
        /// Method to invoke when the Forward command is executed.
        /// </summary>
        private void OnForwardExecute()
        {
            if (!_navigationService.CanGoForward)
            {
                return;
            }

            _navigationService.GoForward();
        }
        #endregion
    }
}