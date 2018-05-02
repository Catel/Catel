// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests
{
    public static class ConsoleHelper
    {
        #region Methods
        public static void Write(string format, params object[] args)
        {
#if NETFX_CORE
            System.Diagnostics.Debug.WriteLine(format, args);
#else
            System.Console.WriteLine(format, args);
#endif
        }
        #endregion
    }
}