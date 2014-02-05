// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateInterceptorCollection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Interceptors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a collection of state interceptors.
    /// </summary>
    public class StateInterceptorCollection : Collection<StateInterceptor>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StateInterceptorCollection"/> class.
        /// </summary>
        public StateInterceptorCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateInterceptorCollection"/> class.
        /// </summary>
        /// <param name="stateInterceptors">The state interceptors.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="stateInterceptors"/> is <c>null</c>.</exception>
        public StateInterceptorCollection(IList<StateInterceptor> stateInterceptors)
            : base(stateInterceptors)
        {
        }
        #endregion
    }
}