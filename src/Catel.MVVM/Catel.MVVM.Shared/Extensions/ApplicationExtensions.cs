
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
        public static Page GetActivePage(this Application @this)
        {
            Argument.IsNotNull(() => @this);

            Page activePage = Application.Current.MainPage;
            while(!(activePage is ContentPage))
            {
                if (activePage is NavigationPage)
                {
                    activePage = ((NavigationPage)activePage).CurrentPage;
                } else  if (activePage is TabbedPage)
                {
                    activePage = ((TabbedPage)activePage).CurrentPage;
                }
            }

            var stack = activePage.Navigation.NavigationStack;
            if (stack.Count > 0)
            {
                activePage = stack[stack.Count - 1];
            }

            return activePage;
        }
    }
}

#endif