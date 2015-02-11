// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

#if NETFX_CORE
using global::Windows.UI.Xaml.Markup;
#elif !PCL && !XAMARIN
using System.Windows.Markup;
#endif

// All other assembly info is defined in SharedAssembly.cs

[assembly: AssemblyTitle("Catel.MVVM")]
[assembly: AssemblyProduct("Catel.MVVM")]
[assembly: AssemblyDescription("Catel MVVM library")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

#if !PCL
[assembly: ComVisible(false)]
#endif

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
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL && !XAMARIN

[assembly: XmlnsPrefix("http://catel.codeplex.com", "catel")]
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.MVVM")]
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.MVVM.Converters")]
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.MVVM.Providers")]
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.MVVM.Views")]
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.Windows")]
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.Windows.Controls")]
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.Windows.Interactivity")]

#if NET
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.Windows.Data")]
#endif

#if NET || SL5
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.Windows.Markup")]
#endif

#if SILVERLIGHT
[assembly: XmlnsDefinition("http://catel.codeplex.com", "Catel.Windows.PopupBehavior")]
#endif

#endif
