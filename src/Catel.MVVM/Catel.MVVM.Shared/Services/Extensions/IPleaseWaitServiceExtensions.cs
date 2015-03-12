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
            Argument.IsNotNull(() => pleaseWaitService);

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
    }
}