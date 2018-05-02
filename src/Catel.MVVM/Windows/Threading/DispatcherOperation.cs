// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherOperation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace System.Windows.Threading
{
    /// <summary>
    /// Class DispatcherOperation.
    /// </summary>
    public class DispatcherOperation
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static DispatcherOperation()
        {
            Default = new DispatcherOperation();
        }

        /// <summary>
        /// Gets the default dispatcher operation.
        /// </summary>
        public static DispatcherOperation Default { get; private set; }
    }
}

#endif