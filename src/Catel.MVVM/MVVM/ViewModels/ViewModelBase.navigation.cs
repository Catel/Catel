namespace Catel.MVVM
{
    using System;
    using Catel.Data;
    using Catel.MVVM.Navigation;

    public partial class ViewModelBase
    {
        private readonly NavigationContext _navigationContext = new NavigationContext();

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        /// <remarks>
        /// Note that the navigation contexts is first available in the <see cref="OnNavigationCompleted"/> method, 
        /// not in the constructor.
        /// </remarks>
        [ExcludeFromValidation]
        protected NavigationContext NavigationContext { get { return _navigationContext; } }

        /// <summary>
        /// Occurs when the navigation is completed.
        /// </summary>
        /// <remarks>
        /// This should of course be a cleaner solution, but there is no other way to let a view-model
        /// know that navigation has completed. Another option is injection, but this would require every
        /// view-model for Windows Phone 7 to accept only the navigation context, which has actually nothing
        /// to do with the logic.
        /// <para />
        /// It is also possible to use the <see cref="OnNavigationCompleted"/> event.
        /// </remarks>
        public event EventHandler NavigationCompleted;

        /// <summary>
        /// Updates the navigation context. The navigation context provided by this class is different
        /// from the <see cref="NavigationContext"/>. Therefore, this method updates the navigation context
        /// to match it to the values of the <paramref name="navigationContext"/>.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public void UpdateNavigationContext(NavigationContext? navigationContext)
        {
            lock (_navigationContext)
            {
                if (navigationContext is not null)
                {
                    foreach (var key in navigationContext.Values.Keys)
                    {
                        _navigationContext.Values[key] = navigationContext.Values[key];
                    }
                }

                NavigationCompleted?.Invoke(this, EventArgs.Empty);

                OnNavigationCompleted();
            }
        }

        /// <summary>
        /// Called when the navigation has completed.
        /// </summary>
        /// <remarks>
        /// This should of course be a cleaner solution, but there is no other way to let a view-model
        /// know that navigation has completed. Another option is injection, but this would require every
        /// view-model for Windows Phone 7 to accept only the navigation context, which has actually nothing
        /// to do with the logic.
        /// <para />
        /// It is also possible to use the <see cref="NavigationCompleted"/> event.
        /// </remarks>
        protected virtual void OnNavigationCompleted() { }
    }
}
