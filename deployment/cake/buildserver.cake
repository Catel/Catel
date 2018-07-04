// Customize this file when using a different build server
#l "buildserver-continuaci.cake"

public string GetBuildServerVariable(string variableName, string defaultValue)
{
    // Just a forwarder, change this line to use a different build server
    return GetContinuaCIVariable(variableName, defaultValue);
}