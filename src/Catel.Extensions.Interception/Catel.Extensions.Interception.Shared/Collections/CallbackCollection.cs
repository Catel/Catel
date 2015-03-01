// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CallbackCollection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Callbacks
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a collection of state callbacks.
    /// </summary>
    public class CallbackCollection : Collection<Callback>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackCollection"/> class.
        /// </summary>
        public CallbackCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CallbackCollection" /> class.
        /// </summary>
        /// <param name="callbacks">The callbacks.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callbacks"/> is <c>null</c>.</exception>
        public CallbackCollection(IList<Callback> callbacks)
            : base(callbacks)
        {
        }
        #endregion
    }
}