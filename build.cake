// Define the required parameters
var DefaultSolutionName = "Catel";
var DefaultCompany = "CatenaLogic";
var DefaultRepositoryUrl = string.Format("https://github.com/{0}/{1}", DefaultSolutionName, DefaultSolutionName);
var StartYear = 2010;

// Note: the rest of the variables should be coming from the build server,
// see `/deployment/cake/*-variables.cake` for customization options

//=======================================================

// Components

var ComponentsToBuild = new string[]
{
    "Catel.Core",
    "Catel.MVVM",
    "Catel.MVVM.Xamarin.Forms",
    "Catel.Serialization.Json",
};

//=======================================================

// WPF apps

var WpfAppsToBuild = new string[]
{

};

//=======================================================

// UWP apps

var UwpAppsToBuild = new string[]
{

};

//=======================================================

// Test projects

var TestProjectsToBuild = new string[]
{
    "Catel.Tests"
};

//=======================================================

// Now all variables are defined, include the tasks, that
// script will take care of the rest of the magic

#l "./deployment/cake/tasks.cake"