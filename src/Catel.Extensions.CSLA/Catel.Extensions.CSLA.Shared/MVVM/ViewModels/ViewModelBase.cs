// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.CSLA
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Auditing;
    using Csla.Xaml;
    using IoC;

    /// <summary>
    /// View model base for CSLA view models with support for Catel.
    /// </summary>
    /// <typeparam name="TModel">The type of the T model.</typeparam>
    [CLSCompliant(false)]
    public abstract class ViewModelBase<TModel> : ViewModel<TModel>, IViewModel, IUniqueIdentifyable
    {
        #region Fields
        /// <summary>
        /// Value indicating whether the view model is already initialized via a call to <see cref="Catel.MVVM.IViewModel.InitializeViewModel" />.
        /// </summary>
        private bool _isViewModelInitialized;

        private readonly IViewModelCommandManager _viewModelCommandManager;

        private EventHandler<EventArgs> _catelInitialized;
        private EventHandler<CommandExecutedEventArgs> _catelCommandExecuted;
        private EventHandler<EventArgs> _catelCanceled;
        private EventHandler<CancelingEventArgs> _catelCanceling;
        private EventHandler<EventArgs> _catelSaved;
        private EventHandler<SavingEventArgs> _catelSaving;
        private EventHandler<EventArgs> _catelClosing;
        private EventHandler<ViewModelClosedEventArgs> _catelClosed;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the view model.
        /// </summary>
        protected ViewModelBase()
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier(GetType());
            ViewModelConstructionTime = DateTime.Now;

            AuditingHelper.RegisterViewModel(this);

            _viewModelCommandManager = ViewModelCommandManager.Create(this);
            _viewModelCommandManager.AddHandler((viewModel, propertyName, command, commandParameter) =>
                _catelCommandExecuted.SafeInvoke(this, new CommandExecutedEventArgs((ICatelCommand)command, commandParameter, propertyName)));

            ViewModelManager.RegisterViewModelInstance(this);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the dependency resolver.
        /// </summary>
        /// <value>The dependency resolver.</value>
        protected IDependencyResolver DependencyResolver { get; private set; }

        /// <summary>
        /// Gets the view model manager.
        /// </summary>
        private ViewModelManager ViewModelManager
        {
            get { return (ViewModelManager)DependencyResolver.Resolve<IViewModelManager>(); }
        }

        /// <summary>
        /// Returns the Catel view model interface of this view model.
        /// </summary>
        public MVVM.IViewModel CatelViewModel
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the unique identifier of the view model.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int UniqueIdentifier { get; private set; }

        /// <summary>
        /// Gets the view model construction time, which is used to get unique instances of view models.
        /// </summary>
        /// <value>The view model construction time.</value>
        public DateTime ViewModelConstructionTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is closed. If a view model is closed, calling
        /// <see cref="Catel.MVVM.IViewModel.CancelViewModel"/>, <see cref="Catel.MVVM.IViewModel.SaveViewModel"/> or 
        /// <see cref="Catel.MVVM.IViewModel.CloseViewModel"/> will have no effect.
        /// </summary>
        /// <value><c>true</c> if the view model is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has a dirty model.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a dirty model; otherwise, <c>false</c>.
        /// </value>
        bool MVVM.IViewModel.HasDirtyModel
        {
            get
            {
                var businessBase = Model as Csla.Core.BusinessBase;
                if (businessBase == null)
                {
                    return false;
                }

                return !businessBase.IsDirty;
            }
        }

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public virtual string Title
        {
            get { return string.Empty; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a command on the view model has been executed.
        /// </summary>
        event EventHandler<EventArgs> MVVM.IViewModel.Initialized
        {
            add { _catelInitialized += value; }
            remove { _catelInitialized -= value; }
        }

        /// <summary>
        /// Occurs when a command on the view model has been executed.
        /// </summary>
        event EventHandler<CommandExecutedEventArgs> MVVM.IViewModel.CommandExecuted
        {
            add { _catelCommandExecuted += value; }
            remove { _catelCommandExecuted -= value; }
        }

        /// <summary>
        /// Occurs when the view model is about to be saved.
        /// </summary>
        event EventHandler<SavingEventArgs> MVVM.IViewModel.Saving
        {
            add { _catelSaving += value; }
            remove { _catelSaving -= value; }
        }

        /// <summary>
        /// Occurs when the view model is saved successfully.
        /// </summary>
        event EventHandler<EventArgs> MVVM.IViewModel.Saved
        {
            add { _catelSaved += value; }
            remove { _catelSaved -= value; }
        }

        /// <summary>
        /// Occurs when the view model is about to be canceled.
        /// </summary>
        event EventHandler<CancelingEventArgs> MVVM.IViewModel.Canceling
        {
            add { _catelCanceling += value; }
            remove { _catelCanceling -= value; }
        }

        /// <summary>
        /// Occurrs when the view model is canceled.
        /// </summary>
        event EventHandler<EventArgs> MVVM.IViewModel.Canceled
        {
            add { _catelCanceled += value; }
            remove { _catelCanceled -= value; }
        }

        /// <summary>
        /// Occurs when the view model is being closed.
        /// </summary>
        event EventHandler<EventArgs> MVVM.IViewModel.Closing
        {
            add { _catelClosing += value; }
            remove { _catelClosing -= value; }
        }

        /// <summary>
        /// Occurs when the view model is being closed.
        /// </summary>
        event EventHandler<ViewModelClosedEventArgs> MVVM.IViewModel.Closed
        {
            add { _catelClosed += value; }
            remove { _catelClosed -= value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Uses the Catels MessageService to display Csla errors.
        /// </summary>
        /// <param name="error"></param>
        protected override void OnError(Exception error)
        {
            var message = new System.Text.StringBuilder();
            if (error != null)
            {
                var ex = error;
                do
                {
                    message.AppendLine(ex.Message).AppendLine();
                    ex = ex.InnerException;
                } while (ex != null);

                message.AppendLine("StackTrace:").AppendLine(error.StackTrace);
            }
            else
            {
                message.Append("Whoops, something went wrong...");
            }

            var dependencyResolver = this.GetDependencyResolver();
            var messageService = dependencyResolver.Resolve<Catel.Services.IMessageService>();
            messageService.ShowError(message.ToString());

            base.OnError(error);
        }

        /// <summary>
        /// Initializes the view model. Normally the initialization is done in the constructor, but sometimes this must be delayed
        /// to a state where the associated UI element (user control, window, ...) is actually loaded.
        /// <para />
        /// This method is called as soon as the associated UI element is loaded.
        /// </summary>
        /// <remarks>It's not recommended to implement the initialization of properties in this method. The initialization of properties
        /// should be done in the constructor. This method should be used to start the retrieval of data from a web service or something
        /// similar.
        /// <para />
        /// During unit tests, it is recommended to manually call this method because there is no external container calling this method.</remarks>
        async Task MVVM.IViewModel.InitializeViewModel()
        {
            if (_isViewModelInitialized)
            {
                return;
            }

            await Initialize();

            _isViewModelInitialized = true;

            _catelInitialized.SafeInvoke(this);
        }

        /// <summary>
        /// Validates the specified notify changed properties only.
        /// </summary>
        /// <param name="force">if set to <c>true</c>, a validation is forced (even if the object knows it is already validated).</param><param name="notifyChangedPropertiesOnly">if set to <c>true</c> only the properties for which the warnings or errors have been changed
        ///             will be updated via <see cref="E:System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>; otherwise all the properties that
        ///             had warnings or errors but not anymore and properties still containing warnings or errors will be updated.</param>
        /// <returns>
        /// <c>true</c> if validation succeeds; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is useful when the view model is initialized before the window, and therefore WPF does not update the errors and warnings.
        /// </remarks>
        bool MVVM.IViewModel.ValidateViewModel(bool force, bool notifyChangedPropertiesOnly)
        {
            var businessBase = Model as Csla.Core.BusinessBase;
            if (businessBase == null)
            {
                return true;
            }

            return businessBase.IsValid;
        }

        /// <summary>
        /// Cancels the editing of the data.
        /// </summary>
        async Task<bool> MVVM.IViewModel.CancelViewModel()
        {
            return await Task.Factory.StartNew(() =>
            {
                if (IsClosed)
                {
                    return false;
                }

                var cancelingEventArgs = new CancelingEventArgs();
                _catelCanceling.SafeInvoke(this, cancelingEventArgs);

                if (cancelingEventArgs.Cancel)
                {
                    return false;
                }

                if (base.CanCancel)
                {
                    base.DoCancel();
                }

                _catelCanceled.SafeInvoke(this);

                return true;
            });
        }

        /// <summary>
        /// Cancels the editing of the data, but also closes the view model in the same call.
        /// </summary>
        async Task<bool> MVVM.IViewModel.CancelAndCloseViewModel()
        {
            if (IsClosed)
            {
                return true;
            }

            if (!await CatelViewModel.CancelViewModel())
            {
                return false;
            }

            await CatelViewModel.CloseViewModel(false);

            return true;
        }

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <returns>
        /// <c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        async Task<bool> MVVM.IViewModel.SaveViewModel()
        {
            return await Task.Factory.StartNew(() =>
            {
                if (IsClosed)
                {
                    return false;
                }

                if (!base.CanSave)
                {
                    return false;
                }

                var e = new SavingEventArgs();
                _catelSaving.SafeInvoke(this, e);
                if (e.Cancel)
                {
                    return false;
                }

                base.Save(this, new ExecuteEventArgs());

                _catelSaved.SafeInvoke(this);

                // Was original call, but not supported in SL
                //base.DoSave();
                return true;
            });
        }

        /// <summary>
        /// Saves the data, but also closes the view model in the same call if the save succeeds.
        /// </summary>
        /// <returns>
        /// <c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        async Task<bool> MVVM.IViewModel.SaveAndCloseViewModel()
        {
            if (IsClosed)
            {
                return false;
            }

            if (!await CatelViewModel.SaveViewModel())
            {
                return false;
            }

            await CatelViewModel.CloseViewModel(true);
            return true;
        }

        /// <summary>
        /// Closes this instance. Always called after the <see cref="M:Catel.MVVM.IViewModel.CancelViewModel"/> of <see cref="M:Catel.MVVM.IViewModel.SaveViewModel"/> method.
        /// </summary>
        /// <param name="result">The result to pass to the view. This will, for example, be used as <c>DialogResult</c>.</param>
        async Task MVVM.IViewModel.CloseViewModel(bool? result)
        {
            await Task.Factory.StartNew(() =>
            {
                if (IsClosed)
                {
                    return;
                }

                _catelClosing.SafeInvoke(this);

                IsClosed = true;

                ViewModelManager.UnregisterViewModelInstance(this);

                _catelClosed.SafeInvoke(this, new ViewModelClosedEventArgs(this, result));
            });
        }

        /// <summary>
        /// Initializes the view model. Normally the initialization is done in the constructor, but sometimes this must be delayed
        /// to a state where the associated UI element (user control, window, ...) is actually loaded.
        /// <para />
        /// This method is called as soon as the associated UI element is loaded.
        /// </summary>
        /// <returns>Task.</returns>
        /// <remarks>It's not recommended to implement the initialization of properties in this method. The initialization of properties
        /// should be done in the constructor. This method should be used to start the retrieval of data from a web service or something
        /// similar.
        /// <para />
        /// During unit tests, it is recommended to manually call this method because there is no external container calling this method.</remarks>
        protected virtual async Task Initialize()
        {
        }

        /// <summary>
        /// Registers the default view model services.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        protected void RegisterViewModelServices(IServiceLocator serviceLocator)
        {
        }
        #endregion
    }
}