#addin nuget:?package=MagicChunks&version=2.0.0.119

using System.Runtime.InteropServices;

[DllImport("kernel32.dll", CharSet=CharSet.Unicode)]
static extern uint GetPrivateProfileString(
   string lpAppName, 
   string lpKeyName,
   string lpDefault, 
   StringBuilder lpReturnedString, 
   uint nSize,
   string lpFileName);

//-------------------------------------------------------------

public string GetContinuaCIVariable(string variableName, string defaultValue)
{
    var argumentValue = Argument(variableName, "non-existing");
    if (argumentValue != "non-existing")
    {
        Information("Variable '{0}' is specified via an argument", variableName);
    
        return argumentValue;
    }

    if (ContinuaCI.IsRunningOnContinuaCI)
    {
        var buildServerVariables = ContinuaCI.Environment.Variable;
        if (buildServerVariables.ContainsKey(variableName))
        {
            Information("Variable '{0}' is specified via Continua CI", variableName);
        
            return buildServerVariables[variableName];
        }
    }
    
    var overrideFile = "./build.cakeoverrides";
    if (System.IO.File.Exists(overrideFile))
    {
        var sb = new StringBuilder(string.Empty, 128);
        var lengthRead = GetPrivateProfileString("General", variableName, null, sb, (uint)sb.Capacity, overrideFile);
        if (lengthRead > 0)
        {
            Information("Variable '{0}' is specified via build.cakeoverrides", variableName);
        
            return sb.ToString();
        }
    }
    
    if (HasEnvironmentVariable(variableName))
    {
        Information("Variable '{0}' is specified via an environment variable", variableName);
    
        return EnvironmentVariable(variableName);
    }
    
    Information("Variable '{0}' is not specified, returning default value", variableName);    
    
    return defaultValue;
}