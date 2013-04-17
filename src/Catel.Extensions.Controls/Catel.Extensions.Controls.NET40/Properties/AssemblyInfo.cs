// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;

#if !NETFX_CORE
using System.Windows;
using System.Windows.Markup;
#endif

// All other assembly info is defined in SharedAssembly.cs

[assembly: AssemblyTitle("Catel.Extensions.Controls")]
[assembly: AssemblyProduct("Catel.Extensions.Controls")]
[assembly: AssemblyDescription("Catel controls library")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// Theme info
#if NET
[assembly: ThemeInfo(
    ResourceDictionaryLocation.SourceAssembly, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
    )]
#endif

// XmlnsDefinition is not supported in Windows Phone 7 and WinRT
#if !WINDOWS_PHONE && !NETFX_CORE

[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.Windows")]
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.Windows.Controls")]
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.Windows.Media.Effects")]

#endif