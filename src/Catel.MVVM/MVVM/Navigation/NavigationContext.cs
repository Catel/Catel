// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Navigation
{
    using System.Collections.Generic;

    /// <summary>
    /// Generic navigation context class that works for all target frameworks.
    /// </summary>
    public class NavigationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationContext"/> class.
        /// </summary>
        public NavigationContext()
        {
            Values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public Dictionary<string, object> Values { get; private set; }
    }
}