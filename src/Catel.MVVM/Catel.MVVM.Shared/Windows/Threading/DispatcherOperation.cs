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
        static DispatcherOperation()
        {
            Default = new DispatcherOperation();
        }

        public static DispatcherOperation Default { get; private set; }
    }
}

#endif