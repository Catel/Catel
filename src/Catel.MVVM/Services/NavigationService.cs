namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Security.Policy;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Catel.Reflection;
    using Logging;

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public partial class NavigationService : NavigationServiceBase, INavigationService
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The registered uris.
        /// </summary>
        private static readonly Dictionary<string, string> RegisteredUris = new Dictionary<string, string>();

        /// <summary>
        /// The navigation root service.
        /// </summary>
        protected readonly INavigationRootService NavigationRootService;
        protected readonly IUrlLocator UrlLocator;

        public NavigationService(INavigationRootService navigationRootService, IUrlLocator urlLocator)
        {
            ArgumentNullException.ThrowIfNull(navigationRootService);
            ArgumentNullException.ThrowIfNull(urlLocator);

            NavigationRootService = navigationRootService;
            UrlLocator = urlLocator;

            Initialize();
        }

        /// <summary>
        /// Occurs when the application is about to be closed.
        /// </summary>
        public event EventHandler<ApplicationClosingEventArgs>? ApplicationClosing;

        /// <summary>
        /// Occurs when nothing has canceled the application closing and the application is really about to be closed.
        /// </summary>
        public event EventHandler<EventArgs>? ApplicationClosed;

        partial void Initialize();

        /// <summary>
        /// Closes the current application. The actual implementation depends on the final target framework.
        /// </summary>
        /// <returns><c>true</c> if the application is closed; otherwise <c>false</c>.</returns>
        public async Task<bool> CloseApplicationAsync()
        {
            var eventArgs = new ApplicationClosingEventArgs();
            ApplicationClosing?.Invoke(this, eventArgs);
            if (eventArgs.Cancel)
            {
                Log.Info("Closing of application is canceled");
                return false;
            }

            await CloseMainWindowAsync();

#pragma warning disable 162
            ApplicationClosed?.Invoke(this, EventArgs.Empty);
            return true;
#pragma warning restore 162
        }

        /// <summary>
        /// Navigates back to the previous page.
        /// </summary>
        public virtual async Task GoBackAsync()
        {
            if (CanGoBack)
            {
                await NavigateBackAsync();
            }
        }

        /// <summary>
        /// Navigates forward to the next page.
        /// </summary>
        public virtual async Task GoForwardAsync()
        {
            if (CanGoForward)
            {
                await NavigateForwardAsync();
            }
        }

        /// <summary>
        /// Navigates to a specific location.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        public virtual Task NavigateAsync(Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            return NavigateToUriAsync(uri);
        }

        /// <summary>
        /// Navigates to a specific location.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter, 
        /// and the value is the value of the parameter.</param>
        /// <exception cref="ArgumentException">The <paramref name="uri"/> is <c>null</c> or whitespace.</exception>
        public virtual Task NavigateAsync(string uri, Dictionary<string, object>? parameters = null)
        {
            Argument.IsNotNullOrWhitespace("uri", uri);

            if (parameters is null)
            {
                parameters = new Dictionary<string, object>();
            }

            return NavigateWithParametersAsync(uri, parameters);
        }

        /// <summary>
        /// Navigates the specified location registered using the view model type.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter, 
        /// and the value is the value of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        public virtual async Task NavigateAsync(Type viewModelType, Dictionary<string, object>? parameters = null)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            var viewModelTypeName = viewModelType.GetSafeFullName();
            string? uri = null;

            lock (RegisteredUris)
            {
                if (!RegisteredUris.TryGetValue(viewModelTypeName, out uri))
                {
                    uri = ResolveNavigationTarget(viewModelType);

                    if (uri is not null)
                    {
                        RegisteredUris.Add(viewModelTypeName, uri);
                    }
                }
            }

            if (uri is null)
            {
                throw Log.ErrorAndCreateException<CatelException>($"Cannot navigate to '{viewModelType.GetSafeFullName()}', could not resolve the uri");
            }

            await NavigateAsync(uri, parameters);
        }

        /// <summary>
        /// Registers the specified view model and the uri. Use this method to override the uri
        /// detection mechanism in Catel.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="uri">The URI to register.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType"/> does not implement <see cref="IViewModel"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        public virtual void Register(Type viewModelType, Uri uri)
        {
            Argument.ImplementsInterface("viewModelType", viewModelType, typeof(IViewModel));
            ArgumentNullException.ThrowIfNull(uri);

            Register(viewModelType.GetSafeFullName(), uri);
        }

        /// <summary>
        /// Registers the specified view model and the uri. Use this method to override the uri
        /// detection mechanism in Catel.
        /// </summary>
        /// <param name="name">The name of the registered page.</param>
        /// <param name="uri">The URI to register.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="name"/> is already registered.</exception>
        public virtual void Register(string name, Uri uri)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            ArgumentNullException.ThrowIfNull(uri);

            lock (RegisteredUris)
            {
                if (RegisteredUris.ContainsKey(name))
                {
                    throw new Exception(Catel.ResourceHelper.GetString("ViewModelAlreadyRegistered"));
                }

                RegisteredUris.Add(name, uri.ToString());

                Log.Debug("Registered view model '{0}' in combination with '{1}' in the NavigationService", name, uri);
            }
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model to unregister.</param>
        /// <returns>
        /// <c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        public virtual bool Unregister(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return Unregister(viewModelType.GetSafeFullName());
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="name">Name of the registered page.</param>
        /// <returns>
        /// <c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        public virtual bool Unregister(string name)
        {
            lock (RegisteredUris)
            {
                var result = RegisteredUris.Remove(name);
                if (result)
                {
                    Log.Debug("Unregistered view model '{0}' in NavigationService", name);
                }

                return result;
            }
        }
    }
}
