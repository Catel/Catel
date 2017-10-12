
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

            Page currentPage = Application.Current.MainPage;
            var modalStack = Application.Current.MainPage.Navigation.ModalStack;
            if (modalStack.Count > 0)
            {
                currentPage = modalStack[modalStack.Count - 1];
            }

            return  (currentPage as NavigationPage)?.CurrentPage ?? currentPage;
        }
    }
}

#endif