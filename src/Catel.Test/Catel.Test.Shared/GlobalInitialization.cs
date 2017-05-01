// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalInitialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Linq;
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
        //System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

        // Required since we do multithreaded initialization
        TypeCache.InitializeTypes(allowMultithreadedInitialization: false);
    }
}