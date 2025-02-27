﻿namespace Catel.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using MVVM.Providers;
    using MVVM.Views;
    using MVVM;
    using System.Windows;
    using UIEventArgs = System.EventArgs;
    using Catel.Services;

    /// <summary>
    /// <see cref="Page"/> class that supports MVVM with Catel.
    /// </summary>
    public class Page : System.Windows.Controls.Page, IPage
    {
        private readonly PageLogic _logic;

        private event EventHandler<EventArgs> _viewLoaded;
        private event EventHandler<EventArgs> _viewUnloaded;
        private event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs> _viewDataContextChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <remarks>It is not possible to inject view models.</remarks>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Page(IServiceProvider serviceProvider, INavigationRootService navigationRootService, IDataContextSubscriptionService dataContextSubscriptionService)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            _logic = new PageLogic(serviceProvider, navigationRootService, this);
            _logic.TargetViewPropertyChanged += (sender, e) =>
            {
                // Do not call this for ActualWidth and ActualHeight WPF, will cause problems with NET 40 
                // on systems where NET45 is *not* installed
                if (!string.Equals(e.PropertyName, nameof(ActualWidth), StringComparison.InvariantCulture) &&
                    !string.Equals(e.PropertyName, nameof(ActualHeight), StringComparison.InvariantCulture))
                {
                    PropertyChanged?.Invoke(this, e);
                }
            };

            _logic.ViewModelChanged += (sender, e) =>
            {
                RaiseViewModelChanged();
            };

            _logic.ViewModelPropertyChanged += (sender, e) =>
            {
                OnViewModelPropertyChanged(e);

                ViewModelPropertyChanged?.Invoke(this, e);
            };

            Loaded += (sender, e) =>
            {
                _viewLoaded?.Invoke(this, EventArgs.Empty);

                OnLoaded(e);
            };

            Unloaded += (sender, e) =>
            {
                _viewUnloaded?.Invoke(this, EventArgs.Empty);

                OnUnloaded(e);
            };

            this.AddDataContextChangedHandler((sender, e) => _viewDataContextChanged?.Invoke(this, new Catel.MVVM.Views.DataContextChangedEventArgs(e.OldValue, e.NewValue)),
                dataContextSubscriptionService);
        }

        /// <summary>
        /// Gets the type of the view model that this user control uses.
        /// </summary>
        public Type ViewModelType
        {
            get { return _logic.GetValue<PageLogic, Type>(x => x.ViewModelType); }
        }

        /// <summary>
        /// Gets the view model that is contained by the container.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel? ViewModel
        {
            get { return _logic.GetValue<PageLogic, IViewModel?>(x => x.ViewModel); }
        }

        /// <summary>
        /// Gets or sets a the view model lifetime management.
        /// <para />
        /// By default, this value is <see cref="ViewModelLifetimeManagement"/>.
        /// </summary>
        /// <value>
        /// The view model lifetime management.
        /// </value>
        public ViewModelLifetimeManagement ViewModelLifetimeManagement
        {
            get { return _logic.GetValue<PageLogic, ViewModelLifetimeManagement>(x => x.ViewModelLifetimeManagement); }
            set { _logic.SetValue<PageLogic>(x => x.ViewModelLifetimeManagement = value); }
        }

        /// <summary>
        /// Occurs when a property on the container has changed.
        /// </summary>
        /// <remarks>
        /// This event makes it possible to externally subscribe to property changes of a <see cref="DependencyObject"/>
        /// (mostly the container of a view model) because the .NET Framework does not allows us to.
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Occurs when the <see cref="ViewModel"/> property has changed.
        /// </summary>
        public event EventHandler<EventArgs>? ViewModelChanged;

        /// <summary>
        /// Occurs when a property on the <see cref="ViewModel"/> has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs>? ViewModelPropertyChanged;

        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        event EventHandler<EventArgs>? IView.Loaded
        {
            add { _viewLoaded += value; }
            remove { _viewLoaded -= value; }
        }

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        event EventHandler<EventArgs>? IView.Unloaded
        {
            add { _viewUnloaded += value; }
            remove { _viewUnloaded -= value; }
        }

        /// <summary>
        /// Occurs when the data context has changed.
        /// </summary>
        event EventHandler<Catel.MVVM.Views.DataContextChangedEventArgs>? IView.DataContextChanged
        {
            add { _viewDataContextChanged += value; }
            remove { _viewDataContextChanged -= value; }
        }

        private void RaiseViewModelChanged()
        {
            OnViewModelChanged();

            ViewModelChanged?.Invoke(this, EventArgs.Empty);
            RaisePropertyChanged(nameof(ViewModel));

            if (_logic.HasVmProperty)
            {
                RaisePropertyChanged("VM");
            }
        }

        /// <summary>
        /// Raises the <c>PropertyChanged</c> event.
        /// </summary>
        /// <param name="propertyName">The property name to raise the event for.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when the page is loaded.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnLoaded(UIEventArgs e)
        {
        }

        /// <summary>
        /// Called when the page is unloaded.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnloaded(UIEventArgs e)
        {
        }

        /// <summary>
        /// Called when a dependency property on this control has changed.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when a property on the current <see cref="ViewModel"/> has changed.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when the <see cref="ViewModel"/> has changed.
        /// </summary>
        /// <remarks>
        /// This method does not implement any logic and saves a developer from subscribing/unsubscribing
        /// to the <see cref="ViewModelChanged"/> event inside the same user control.
        /// </remarks>
        protected virtual void OnViewModelChanged()
        {
        }
    }
}
