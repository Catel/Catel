// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherOperation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if UWP

namespace System.Windows.Threading
{
    using global::Windows.UI.Core;

    /// <summary>
    /// Dispatcher priority enum for platform compatibility.
    /// </summary>
    public enum DispatcherPriority
    {
        ApplicationIdle = 2,

        Background = 4,

        ContextIdle = 3,

        DataBind = 8,

        Inactive = 0,

        Input = 5,

        Invalid = -1,

        Loaded = 6,

        Normal = 9,

        Render = 7,

        Send = 10,

        SystemIdle = 1
    }

    public static class DispatcherPriorityExtensions
    {
        public static CoreDispatcherPriority ToCoreDispatcherPriority(this DispatcherPriority dispatcherPriority)
        {
            var intValue = (int)dispatcherPriority;
            if (intValue <= (int)DispatcherPriority.ContextIdle)
            {
                return CoreDispatcherPriority.Idle;
            }

            if (intValue <= (int)DispatcherPriority.Render)
            {
                return CoreDispatcherPriority.Low;
            }

            if (intValue <= (int)DispatcherPriority.Normal)
            {
                return CoreDispatcherPriority.Normal;
            }

            return CoreDispatcherPriority.High;
        }
    }
}

#endif
