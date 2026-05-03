namespace Catel.Services;

using System;
using System.Threading.Tasks;

/// <summary>
/// Extension methods for the <see cref="IDispatcherService"/>.
/// </summary>
public static class IDispatcherServiceExtensions
{
    /// <summary>
    /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
    /// <para />
    /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
    /// </summary>
    /// <param name="dispatcherService">The dispatcher service.</param>
    /// <param name="action">The action.</param>
    /// <returns>The task representing the action.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
    public static Task InvokeIfRequiredAsync(this IDispatcherService dispatcherService, Action action)
    {
        return dispatcherService.InvokeAsync(action);
    }

    /// <summary>
    /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
    /// <para />
    /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
    /// </summary>
    /// <param name="dispatcherService">The dispatcher service.</param>
    /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
    /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
    /// <returns>The task representing the action.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
    public static Task InvokeIfRequiredAsync(this IDispatcherService dispatcherService, Delegate method, params object[] args)
    {
        return dispatcherService.InvokeAsync(method, args);
    }

    /// <summary>
    /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
    /// </summary>
    /// <param name="dispatcherService">The dispatcher service.</param>
    /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
    /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
    /// <returns>The task representing the action.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
    public static Task BeginInvokeAsync(this IDispatcherService dispatcherService, Delegate method, params object[] args)
    {
        return dispatcherService.InvokeAsync(method, args);
    }

    /// <summary>
    /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on.
    /// </summary>
    /// <param name="dispatcherService">The dispatcher service.</param>
    /// <param name="action">The action.</param>
    /// <returns>The task representing the action.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
    public static Task BeginInvokeAsync(this IDispatcherService dispatcherService, Action action)
    {
        return dispatcherService.InvokeAsync(action);
    }

    /// <summary>
    /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
    /// <para />
    /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
    /// </summary>
    /// <param name="dispatcherService">The dispatcher service.</param>
    /// <param name="action">The action.</param>
    /// <returns>The task representing the action.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
    public static Task BeginInvokeIfRequiredAsync(this IDispatcherService dispatcherService, Action action)
    {
        return dispatcherService.InvokeAsync(action);
    }

    /// <summary>
    /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
    /// <para />
    /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
    /// </summary>
    /// <param name="dispatcherService">The dispatcher service.</param>
    /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
    /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
    /// <returns>The task representing the action.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
    public static Task BeginInvokeIfRequiredAsync(this IDispatcherService dispatcherService, Delegate method, params object[] args)
    {
        return dispatcherService.InvokeAsync(method, args);
    }
}
