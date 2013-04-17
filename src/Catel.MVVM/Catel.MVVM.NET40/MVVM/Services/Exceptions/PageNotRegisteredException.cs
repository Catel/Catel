// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageNotRegisteredException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;

    /// <summary>
    /// Exception in case a page not registered, but still being used.
    /// </summary>
    public class PageNotRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageNotRegisteredException"/> class.
        /// </summary>
        /// <param name="name">The name of the page.</param>
        public PageNotRegisteredException(string name)
            : base(string.Format(ResourceHelper.GetString(typeof(PageNotRegisteredException), "Exceptions", "PageNotRegistered"), name))
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the page.
        /// </summary>
        /// <value>The name of the page.</value>
        public string Name { get; private set; }
    }
}
