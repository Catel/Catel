public void SetContinuaCIVersion(string version)
{
    if (ContinuaCI.IsRunningOnContinuaCI)
    {
        Information("Setting version '{0}' in Continua CI", version);

        var message = string.Format("@@continua[setBuildVersion value='{0}']", version);
        WriteContinuaCiIntegration(message);
    }
}

//-------------------------------------------------------------

public void SetContinuaCIVariable(string variableName, string value)
{
    if (ContinuaCI.IsRunningOnContinuaCI)
    {
        Information("Setting variable '{0}' to '{1}' in Continua CI", variableName, value);
    
        var message = string.Format("@@continua[setVariable name='{0}' value='{1}' skipIfNotDefined='true']", variableName, value);
        WriteContinuaCiIntegration(message);
    }
}

//-------------------------------------------------------------

public Tuple<bool, string> GetContinuaCIVariable(string variableName, string defaultValue)
{
    var exists = false;
    var value = string.Empty;

    if (ContinuaCI.IsRunningOnContinuaCI)
    {
        var buildServerVariables = ContinuaCI.Environment.Variable;
        if (buildServerVariables.ContainsKey(variableName))
        {
            Information("Variable '{0}' is specified via Continua CI", variableName);
        
            exists = true;
            value = buildServerVariables[variableName];
        }
    }

    return new Tuple<bool, string>(exists, value);
}

//-------------------------------------------------------------

private void WriteContinuaCiIntegration(string message)
{
    // Must be Console.WriteLine
    Information(message);
}