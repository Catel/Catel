// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Repositories
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface defining basic repository behavior.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public interface IRepository<TModel>
    {
        /// <summary>
        /// Gets the data. 
        /// <para />
        /// If the data is already loaded, it will immediately return.
        /// <para />
        /// If the data is currently loading, the handler will be queued and called as soon as the data is loaded.
        /// </summary>
        /// <param name="completed">The handler to call when the data is retrieved, can be <c>null</c>.</param>
        void GetData(Action<IEnumerable<TModel>> completed = null);
    }
}