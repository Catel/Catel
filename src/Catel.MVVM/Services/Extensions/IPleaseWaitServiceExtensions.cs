// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPleaseWaitServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;

    /// <summary>
    /// IPleaseWaitService extensions.
    /// </summary>
    public static class IPleaseWaitServiceExtensions
    {
        /// <summary>
        /// Hides the please wait service temporarily by storing the show counter and restoring it afterwards.
        /// </summary>
        /// <returns>IDisposable.</returns>
        public static IDisposable HideTemporarily(this IPleaseWaitService pleaseWaitService)
        {
            Argument.IsNotNull("pleaseWaitService", pleaseWaitService);

            var showCounter = pleaseWaitService.ShowCounter;

            return new DisposableToken<IPleaseWaitService>(pleaseWaitService,
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
        /// Calls <see cref="IPleaseWaitService.Push"/> and returns a disposable token. As soon as the token is disposed, it will
        /// call <see cref="IPleaseWaitService.Pop"/>.
        /// <para />
        /// This is a great way to safely show a busy indicator and ensure that the indicator hides, even when an exception occurs.
        /// </summary>
        /// <param name="pleaseWaitService">The please wait service.</param>
        /// <param name="status">The status to change the text to.</param>
        /// <returns>IDisposable.</returns>
        /// <example>
        /// <![CDATA[
        /// using (pleaseWaitService.PushInScope())
        /// {
        ///     // some code that might throw exceptions
        /// }
        /// ]]>
        /// </example>
        public static IDisposable PushInScope(this IPleaseWaitService pleaseWaitService, string status = "")
        {
            Argument.IsNotNull("pleaseWaitService", pleaseWaitService);

            return new DisposableToken<IPleaseWaitService>(pleaseWaitService, token => token.Instance.Push(status), token => token.Instance.Pop());
        }
    }
}