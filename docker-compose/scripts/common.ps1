param(
[Parameter(Mandatory=$true)][String]$environmentName,
[Parameter(Mandatory=$true)][String]$environmentVariablesPath)

[Environment]::SetEnvironmentVariable("COMPOSE_HTTP_TIMEOUT", 120)
[Environment]::SetEnvironmentVariable("DOCKER_CLIENT_TIMEOUT", 120)

$environmentVariables = import-csv $environmentVariablesPath -Delimiter '=' -Header Name,Value
Foreach ($environmentVariable in $environmentVariables)
{
    [Environment]::SetEnvironmentVariable($environmentVariable.Name.Trim(), $environmentVariable.Value.Trim())
}