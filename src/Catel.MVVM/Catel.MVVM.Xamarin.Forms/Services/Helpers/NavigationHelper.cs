// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System.Threading.Tasks;
    using global::Xamarin.Forms;

    /// <summary>
    /// The navigation helper class.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// Presents a <see cref="T:Xamarin.Forms.Page"/> modally.
        /// </summary>
        /// <param name="contentPage">The page to present</param>
        /// <returns>
        /// An awaitable Task, indicating the PushModal completion.
        /// </returns>
        public static Task PushModalAsync(ContentPage contentPage)
        {
            return Application.Current.GetActivePage().Navigation.PushModalAsync(contentPage);
        }

        /// <summary>
        /// Asynchronously dismisses the most recent modally presented <see cref="T:Xamarin.Forms.Page"/>.
        /// </summary>
        /// <returns>
        /// An awaitable Task&lt;Page&gt;, indicating the PopModalAsync completion. The Task.Result is the Page that has been popped.
        /// </returns>
        public static Task PopModalAsync()
        {
            return Application.Current.GetActivePage().Navigation.PopModalAsync();
        }
    }
}