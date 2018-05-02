// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Application.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Android.App
{
    using System;
    using global::Android.Runtime;

    /// <summary>
    /// Application class to be able to have a static context.
    /// </summary>
    public class Application : global::Android.App.Application
    {
        /// <summary>
        /// The constructor of the application class.
        /// </summary>
        /// <param name="javaReference"></param>
        /// <param name="transfer"></param>
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            if (Default != null)
            {
                Default = this;
            }
        }

        /// <summary>
        /// Gets the default application instance.
        /// </summary>
        /// <value>The default.</value>
        public static Application Default { get; private set; }
    }
}