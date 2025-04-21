//=======================================================
// DEFINE PARAMETERS
//=======================================================

// Define the required parameters
var Parameters = new Dictionary<string, object>();
Parameters["SolutionName"] = "Catel";
Parameters["Company"] = "CatenaLogic";
Parameters["RepositoryUrl"] = "https://github.com/catel/catel";
Parameters["StartYear"] = "2010";
Parameters["UseVisualStudioPrerelease"] = "true";

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

// All catel libraries are  a dependency of the test references
Dependencies.Add("Catel.Core");
Dependencies.Add("Catel.MVVM");
Dependencies.Add("Catel.Tests.TestReferenceB", new [] 
{
    "Catel.Tests.TestReferenceA",
    "Catel.Tests"
});
Dependencies.Add("Catel.Tests.TestReferenceC", new [] 
{
    "Catel.Tests.TestReferenceA",
    "Catel.Tests"
});
Dependencies.Add("Catel.Tests.TestReferenceA", new [] 
{
    "Catel.Tests"
});
Dependencies.Add("Catel.Tests");

Components.Add("Catel.Core");
Components.Add("Catel.MVVM");

TestProjects.Add("Catel.Tests");

//=======================================================
// REQUIRED INITIALIZATION, DO NOT CHANGE
//=======================================================

// Now all variables are defined, include the tasks, that
// script will take care of the rest of the magic

#l "./deployment/cake/tasks.cake"
