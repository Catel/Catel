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
	
	if (HasEnvironmentVariable(variableName))
	{
		Information("Variable '{0}' is specified via an environment variable", variableName);
	
		return EnvironmentVariable(variableName);
	}
	
	Information("Variable '{0}' is not specified, returning default value", variableName);	
	
	return defaultValue;
}