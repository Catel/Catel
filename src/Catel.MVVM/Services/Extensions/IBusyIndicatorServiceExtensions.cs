namespace Catel.Services
{
    using System;

    public static class IBusyIndicatorServiceExtensions
    {
        /// <summary>
        /// Hides the busy indicator service temporarily by storing the show counter and restoring it afterwards.
        /// </summary>
        /// <returns>IDisposable.</returns>
        public static IDisposable HideTemporarily(this IBusyIndicatorService busyIndicatorService)
        {
            Argument.IsNotNull("busyIndicatorService", busyIndicatorService);

            var showCounter = busyIndicatorService.ShowCounter;

            return new DisposableToken<IBusyIndicatorService>(busyIndicatorService,
                x => x.Instance.Hide(),
                x =>
                {
                    for (var i = 0; i < showCounter; i++)
                    {
                        if (x.Instance.ShowCounter < showCounter)
                        {
                            x.Instance.Push();
                        }
                    }
                });
        }

        /// <summary>
        /// Calls <see cref="IBusyIndicatorService.Push"/> and returns a disposable token. As soon as the token is disposed, it will
        /// call <see cref="IBusyIndicatorService.Pop"/>.
        /// <para />
        /// This is a great way to safely show a busy indicator and ensure that the indicator hides, even when an exception occurs.
        /// </summary>
        /// <param name="busyIndicatorService">The please wait service.</param>
        /// <param name="status">The status to change the text to.</param>
        /// <returns>IDisposable.</returns>
        /// <example>
        /// <![CDATA[
        /// using (busyIndicatorService.PushInScope())
        /// {
        ///     // some code that might throw exceptions
        /// }
        /// ]]>
        /// </example>
        public static IDisposable PushInScope(this IBusyIndicatorService busyIndicatorService, string status = "")
        {
            Argument.IsNotNull("busyIndicatorService", busyIndicatorService);

            return new DisposableToken<IBusyIndicatorService>(busyIndicatorService, token => token.Instance.Push(status), token => token.Instance.Pop());
        }
    }
}
