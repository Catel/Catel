// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalInitialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;
using System.Linq;
using Catel.Data;
using Catel.IoC;
using Catel.Logging;
using Catel.Reflection;
using NUnit.Framework;

/// <summary>
/// Sets the current culture to <c>en-US</c> for all unit tests to prevent tests to fail
/// due to cultural string differences.
/// </summary>
[SetUpFixture]
public class GlobalInitialization
{
    [OneTimeSetUp]
    public static void SetUp()
    {
        if (Debugger.IsAttached)
        {
            LogManager.AddDebugListener();
        }

        // For testing purposes, enable features we disabled for CTL-234
        var modelEqualityComparer = ServiceLocator.Default.ResolveType<IModelEqualityComparer>();
        modelEqualityComparer.CompareProperties = true;
        modelEqualityComparer.CompareValues = true;
        modelEqualityComparer.CompareCollections = true;

        //System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        // Required since we do multithreaded initialization
        TypeCache.InitializeTypes(allowMultithreadedInitialization: false);
    }
}