namespace Catel.Services
{
    using System;
    using System.Windows.Controls;
    using System.Windows;

    public static class IWrapControlServiceExtensions
    {
        /// <summary>
        /// Gets a wrapped element by name.
        /// </summary>
        /// <typeparam name="T">Type of the control to return.</typeparam>
        /// <param name="wrapControlService">The wrap control service.</param>
        /// <param name="wrappedGrid">The wrapped grid.</param>
        /// <param name="controlName">Name of the control.</param>
        /// <returns>
        /// 	<see cref="FrameworkElement"/> or <c>null</c> if the element is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="wrappedGrid"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="controlName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="controlName"/> is not a valid control name.</exception>
        public static T? GetWrappedElement<T>(this IWrapControlService wrapControlService, Grid wrappedGrid, string controlName)
            where T : FrameworkElement
        {
            return wrapControlService.GetWrappedElement(wrappedGrid, controlName) as T;
        }

        /// <summary>
        /// Gets a wrapped element mapped by the <paramref name="wrapOption"/>.
        /// </summary>
        /// <typeparam name="T">Type of the control to return.</typeparam>
        /// <param name="wrapControlService">The wrap control service.</param>
        /// <param name="wrappedGrid">The wrapped grid.</param>
        /// <param name="wrapOption">The wrap option that is used, which will be mapped to the control. The value <see cref="WrapControlServiceWrapOptions.All"/> is not allowed and will throw an exception.</param>
        /// <returns>
        /// 	<see cref="FrameworkElement"/> or <c>null</c> if the element is not found.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="wrappedGrid"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="wrapOption"/> is <see cref="WrapControlServiceWrapOptions.All"/>.</exception>
        public static T? GetWrappedElement<T>(this IWrapControlService wrapControlService, Grid wrappedGrid, WrapControlServiceWrapOptions wrapOption)
            where T : FrameworkElement
        {
            return wrapControlService.GetWrappedElement(wrappedGrid, wrapOption) as T;
        }
    }
}
