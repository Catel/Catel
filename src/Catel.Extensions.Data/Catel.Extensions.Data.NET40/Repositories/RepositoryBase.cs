// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Repositories
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for repositories.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public abstract class RepositoryBase<TModel> : IRepository<TModel>
    {
        //private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TModel}" /> class.
        /// </summary>
        protected RepositoryBase()
        {
            
        }

        /// <summary>
        /// Gets the data. 
        /// <para />
        /// If the data is already loaded, it will immediately return.
        /// <para />
        /// If the data is currently loading, the handler will be queued and called as soon as the data is loaded.
        /// </summary>
        /// <param name="completed">The handler to call when the data is retrieved, can be <c>null</c>.</param>
        public abstract void GetData(Action<IEnumerable<TModel>> completed = null);
    }
}