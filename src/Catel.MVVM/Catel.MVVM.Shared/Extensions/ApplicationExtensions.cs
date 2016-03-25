
#if XAMARIN_FORMS

namespace Catel
{
    using global::Xamarin.Forms;

    /// <summary>
    /// The application extension methods.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <param name="this">
        /// The instance.
        /// </param>
        /// <returns>
        /// The current or top most page of the application.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="this"/> is <c>null</c>.</exception>
        public static Page CurrentPage(this Application @this)
        {
            Argument.IsNotNull(() => @this);

            var modalStack = Application.Current.MainPage.Navigation.ModalStack;
            var currentPage = modalStack[modalStack.Count - 1];
            if (currentPage is NavigationPage)
            {
                currentPage = (currentPage as NavigationPage).CurrentPage ?? currentPage;
            }

            return currentPage;
        }
    }
}

#endif