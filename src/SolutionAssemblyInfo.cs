// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionAssemblyInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

//#define STRONGNAME_ASSEMBLIES

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
[assembly: AssemblyCopyright("Copyright © CatenaLogic 2010 - 2017")]
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

[assembly: AssemblyVersion("5.1")]
[assembly: AssemblyInformationalVersion("5.1, manually built in Visual Studio")]

#if DEBUG

#if NET45
[assembly: AssemblyConfiguration("NET45, Debug")]
#elif NET46
[assembly: AssemblyConfiguration("NET46, Debug")]
#elif NET47
[assembly: AssemblyConfiguration("NET47, Debug")]
#elif NET50
[assembly: AssemblyConfiguration("NET50, Debug")]
#elif UWP
[assembly: AssemblyConfiguration("Universal Windows Platform 10.0, Debug")]
#elif PCL
[assembly: AssemblyConfiguration("PCL, Debug")]
#elif XAMARIN && ANDROID
[assembly: AssemblyConfiguration("Xamarin.Android, Debug")]
#elif XAMARIN && IOS
[assembly: AssemblyConfiguration("Xamarin.iOS, Debug")]
#else
[assembly: AssemblyConfiguration("Unknown, Debug")]
#endif

#else

#if NET45
[assembly: AssemblyConfiguration("NET45, Release")]
#elif NET46
[assembly: AssemblyConfiguration("NET46, Release")]
#elif NET47
[assembly: AssemblyConfiguration("NET47, Release")]
#elif NET50
[assembly: AssemblyConfiguration("NET50, Release")]
#elif UWP
[assembly: AssemblyConfiguration("Universal Windows Platform 10.0, Release")]
#elif PCL
[assembly: AssemblyConfiguration("PCL, Release")]
#elif XAMARIN && ANDROID
[assembly: AssemblyConfiguration("Xamarin.Android, Release")]
#elif XAMARIN && IOS
[assembly: AssemblyConfiguration("Xamarin.iOS, Release")]
#else
[assembly: AssemblyConfiguration("Unknown, Release")]
#endif

#endif

//// CLS compliant
//#if !NETFX_CORE && !XAMARIN && !TEST
//[assembly: CLSCompliant(true)]
//#endif

#if STRONGNAME_ASSEMBLIES
// Sign assembly (this is relative to the obj output directory)
#if X86 || X64
[assembly: AssemblyKeyFile(@"..\..\..\..\..\Catel.snk")]
#else
[assembly: AssemblyKeyFile(@"..\..\..\..\Catel.snk")]
#endif

#endif

#if STRONGNAME_ASSEMBLIES
[assembly: InternalsVisibleTo("Catel.Core, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]
[assembly: InternalsVisibleTo("Catel.MVVM, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]
[assembly: InternalsVisibleTo("Catel.Test, PublicKey=002400000480000094000000060200000024000052534131000400000100010099d04b18e032ce24bae6ede7b654e09745bf0c7268b5aac1582bcfb349808123a34748ddfc67c884a03d4b8e4e9377d33ed53d7810973bc80d69335ac8c76f03d6250f5b916e2d8b3107ba83501339a6f94757200fa40c002131dd227fbadbe0331b89a6afd3242c21f88a3abe5d91304d5a26cc3103126f077542278669b5a2")]
#else
[assembly: InternalsVisibleTo("Catel.Core")]
[assembly: InternalsVisibleTo("Catel.MVVM")]

[assembly: InternalsVisibleTo("Catel.Test")]
#endif