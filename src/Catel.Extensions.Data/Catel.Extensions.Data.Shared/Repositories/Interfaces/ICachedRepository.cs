// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICachedRepository.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Repositories
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extended repository with caching support.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public interface ICachedRepository<TModel> : IRepository<TModel>
    {
        /// <summary>
        /// Gets the current data. If <see cref="IsDataLoaded"/> is <c>true</c>, this property will return <c>null</c>.
        /// </summary>
        /// <value>The data.</value>
        IEnumerable<TModel> Data { get; }

        /// <summary>
        /// Gets a value indicating whether this repository is currently loading data.
        /// </summary>
        /// <value><c>true</c> if this repository is loading data; otherwise, <c>false</c>.</value>
        bool IsLoadingData { get; }

        /// <summary>
        /// Gets a value indicating whether this repository has its data loaded.
        /// </summary>
        /// <value><c>true</c> if this repository has its data loaded; otherwise, <c>false</c>.</value>
        bool IsDataLoaded { get; }

        /// <summary>
        /// Gets the data loaded timestamp.
        /// </summary>
        /// <value>The data loaded timestamp.</value>
        DateTime DataLoadedTimestamp { get; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        TimeSpan Expiration { get; set; }
    }
}