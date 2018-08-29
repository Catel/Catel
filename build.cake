//=======================================================
// DEFINE PARAMETERS
//=======================================================

// Define the required parameters
var Parameters = new Dictionary<string, object>();
Parameters["SolutionName"] = "Catel";
Parameters["Company"] = "CatenaLogic";
Parameters["RepositoryUrl"] = string.Format("https://github.com/{0}/{1}", GetBuildServerVariable("SolutionName"), GetBuildServerVariable("SolutionName"));
Parameters["StartYear"] = "2010";

// Note: the rest of the variables should be coming from the build server,
// see `/deployment/cake/*-variables.cake` for customization options
// 
// If required, more variables can be overridden by specifying them via the 
// Parameters dictionary, but the build server variables will always override
// them if defined by the build server. For example, to override the code
// sign wild card, add this to build.cake
//
// Parameters["CodeSignWildcard"] = "Orc.EntityFramework";

//=======================================================
// DEFINE COMPONENTS TO BUILD / PACKAGE
//=======================================================

var ComponentsToBuild = new string[]
{
    "Catel.Core",
    "Catel.MVVM",
    "Catel.MVVM.Xamarin.Forms",
    "Catel.Serialization.Json",
};

//=======================================================
// DEFINE WEB APPS TO BUILD / PACKAGE
//=======================================================

var WebAppsToBuild = new string[]
{

};

//=======================================================
// DEFINE WPF APPS TO BUILD / PACKAGE
//=======================================================

var WpfAppsToBuild = new string[]
{

};

//=======================================================
// DEFINE UWP APPS TO BUILD / PACKAGE
//=======================================================

var UwpAppsToBuild = new string[]
{

};

//=======================================================
// DEFINE TEST PROJECTS TO BUILD
//=======================================================

var TestProjectsToBuild = new string[]
{
    string.Format("{0}.Tests", GetBuildServerVariable("SolutionName"))
};

//=======================================================
// REQUIRED INITIALIZATION, DO NOT CHANGE
//=======================================================

// Now all variables are defined, include the tasks, that
// script will take care of the rest of the magic

#l "./deployment/cake/tasks.cake"
