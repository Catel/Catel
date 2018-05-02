var projectName = "Catel";
var projectsToPackage = new [] 
{ 
    "Catel.Core",
    "Catel.MVVM",
    "Catel.MVVM.Xamarin.Forms",
    "Catel.Serialization.Json",
};
var company = "CatenaLogic";
var startYear = 2010;
var defaultRepositoryUrl = string.Format("https://github.com/{0}/{1}", projectName, projectName);

#l "./deployment/cake/variables.cake"
#l "./deployment/cake/tasks.cake"
