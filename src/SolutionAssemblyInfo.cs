// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionAssemblyInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

//#define SIGN_ASSEMBLIES

#pragma warning disable 1699    // 1699 = Use command line option '/keyfile' or appropriate project settings instead of 'AssemblyKeyFile'

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

// Shared assembly info that is common for all assemblies of this project

////[assembly: AssemblyTitle("DEFINED IN ACTUAL ASSEMBLYINFO")]
////[assembly: AssemblyProduct("DEFINED IN ACTUAL ASSEMBLYINFO")]
////[assembly: AssemblyDescription("DEFINED IN ACTUAL ASSEMBLYINFO")]

[assembly: AssemblyCompany("CatenaLogic")]
[assembly: AssemblyCopyright("Copyright © CatenaLogic 2010 - 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("3.7")]
[assembly: AssemblyInformationalVersion("3.7, Nightly, Released on 2013-09-01 11:53")]

#if DEBUG

#if NET35
[assembly: AssemblyConfiguration("NET35, Debug")]
#elif NET40
[assembly: AssemblyConfiguration("NET40, Debug")]
#elif NET45
[assembly: AssemblyConfiguration("NET45, Debug")]
#elif NET50
[assembly: AssemblyConfiguration("NET45, Debug")]
#elif SL4
[assembly: AssemblyConfiguration("Silverlight 4, Debug")]
#elif SL5
[assembly: AssemblyConfiguration("Silverlight 5, Debug")]
#elif WP7
[assembly: AssemblyConfiguration("Windows Phone 7, Debug")]
#elif WP8
[assembly: AssemblyConfiguration("Windows Phone 8, Debug")]
#elif WIN80
[assembly: AssemblyConfiguration("Windows 8, Debug")]
#else
[assembly: AssemblyConfiguration("Unknown, Debug")]
#endif

#else

#if NET35
[assembly: AssemblyConfiguration("NET35, Release")]
#elif NET40
[assembly: AssemblyConfiguration("NET40, Release")]
#elif NET45
[assembly: AssemblyConfiguration("NET45, Release")]
#elif NET50
[assembly: AssemblyConfiguration("NET45, Release")]
#elif SL4
[assembly: AssemblyConfiguration("Silverlight 4, Release")]
#elif SL5
[assembly: AssemblyConfiguration("Silverlight 5, Release")]
#elif WP7
[assembly: AssemblyConfiguration("Windows Phone 7, Release")]
#elif WP8
[assembly: AssemblyConfiguration("Windows Phone 8, Release")]
#elif WIN80
[assembly: AssemblyConfiguration("Windows 8, Release")]
#else
[assembly: AssemblyConfiguration("Unknown, Release")]
#endif

#endif

// CLS compliant
#if !NETFX_CORE
[assembly: CLSCompliant(true)]
#endif

#if SIGN_ASSEMBLIES
// Do not sign any Reactive Extensions libraries, they are not signed either
#if !RX
// Sign assembly (this is relative to the obj output directory)
#if X86 || X64
[assembly: AssemblyKeyFile(@"..\..\..\..\Catel.snk")]
#else

[assembly: AssemblyKeyFile(@"..\..\..\Catel.snk")]
#endif
#endif
#endif

// Allow unit test to find test

#if SIGN_ASSEMBLIES
[assembly: InternalsVisibleTo("Catel.Core, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]
[assembly: InternalsVisibleTo("Catel.MVVM, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]

[assembly: InternalsVisibleTo("Catel.Extensions.Controls, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]
[assembly: InternalsVisibleTo("Catel.Extensions.CSLA, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]
[assembly: InternalsVisibleTo("Catel.Extensions.FluentValidation, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]
[assembly: InternalsVisibleTo("Catel.Extensions.Prism, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]

[assembly: InternalsVisibleTo("Catel.Test, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]
#else
[assembly: InternalsVisibleTo("Catel.Core")]
[assembly: InternalsVisibleTo("Catel.MVVM")]
[assembly: InternalsVisibleTo("Catel.Mvc")]

[assembly: InternalsVisibleTo("Catel.Extensions.Controls")]
[assembly: InternalsVisibleTo("Catel.Extensions.CSLA")]
[assembly: InternalsVisibleTo("Catel.Extensions.Data")]
[assembly: InternalsVisibleTo("Catel.Extensions.DynamicObjects")]
[assembly: InternalsVisibleTo("Catel.Extensions.EntityFramework5")]
[assembly: InternalsVisibleTo("Catel.Extensions.FluentValidation")]
[assembly: InternalsVisibleTo("Catel.Extensions.Interception")]
[assembly: InternalsVisibleTo("Catel.Extensions.Memento")]
[assembly: InternalsVisibleTo("Catel.Extensions.Prism")]

[assembly: InternalsVisibleTo("Catel.Test")]
#endif