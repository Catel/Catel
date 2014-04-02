// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Application.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Android.App
{
    using global::Android.Content;

    /// <summary>
    /// Application class to be able to have a static context.
    /// </summary>
    public class Application : global::Android.App.Application
    {
        /// <summary>
        /// Initializes static members of the <see cref="Application"/> class.
        /// </summary>
        static Application()
        {
            Default = new Application();
        }

        /// <summary>
        /// Gets the default application instance.
        /// </summary>
        /// <value>The default.</value>
        public static Application Default { get; private set; }
    }
}