// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Android
{
    using global::Android.Content;

    /// <summary>
    /// The context helper.
    /// </summary>
    public static class ContextHelper
    {
        private static Context _context;

        /// <summary>
        /// Gets or sets the current context.
        /// <para />
        /// Note that the setter is made public to allow customization. It will be used primarily by Catel though.
        /// </summary>
        /// <value>The current context.</value>
        public static Context CurrentContext
        {
            get
            {
                if (_context == null)
                {
                    _context = global::Android.App.Application.Context;
                }

                return _context;
            }
            set { _context = value; }
        }
    }
}