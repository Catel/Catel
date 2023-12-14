using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

// All other assembly info is defined in SolutionAssemblyInfo.cs

[assembly: AssemblyTitle("Catel.MVVM")]
[assembly: AssemblyProduct("Catel.MVVM")]
[assembly: AssemblyDescription("Catel MVVM library")]
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: InternalsVisibleTo("Catel.Tests")]

// Theme info
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
    )]

[assembly: XmlnsPrefix("http://schemas.catelproject.com", "catel")]
[assembly: XmlnsDefinition("http://schemas.catelproject.com", "Catel.MVVM")]
[assembly: XmlnsDefinition("http://schemas.catelproject.com", "Catel.MVVM.Converters")]
[assembly: XmlnsDefinition("http://schemas.catelproject.com", "Catel.MVVM.Providers")]
[assembly: XmlnsDefinition("http://schemas.catelproject.com", "Catel.MVVM.Views")]
[assembly: XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows")]
[assembly: XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows.Controls")]
[assembly: XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows.Data")]
[assembly: XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows.Interactivity")]
[assembly: XmlnsDefinition("http://schemas.catelproject.com", "Catel.Windows.Markup")]
