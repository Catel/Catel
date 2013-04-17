// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalInitialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using Catel.Logging;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    /// Sets the current culture to <c>en-US</c> for all unit tests to prevent tests to fail
    /// due to cultural string differences.
    /// </summary>
    [TestClass]
    public class GlobalInitialization
    {
        [AssemblyInitialize]
        public static void InitializeAssembly(TestContext context)
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }
    }
}