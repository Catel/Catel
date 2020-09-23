// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalInitialization.approvaltests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.Windows;

#if DEBUG
[assembly: UseReporter(typeof(BeyondCompareReporter))]
#else
[assembly: UseReporter(typeof(DiffReporter))]
#endif

