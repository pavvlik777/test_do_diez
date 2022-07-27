param(
[String]$environmentName = "localhost",
[String]$environmentVariablesPath = [IO.Path]::Combine("..", "configurations", "localhost.env"))

$commonScriptPath = [IO.Path]::Combine($PSScriptRoot, "common.ps1")
. $commonScriptPath -environmentName $environmentName -environmentVariablesPath $environmentVariablesPath

$dockerComposePath = [IO.Path]::Combine("..", "configurations", "docker-compose.yml")
$dockerComposeServicesPath = [IO.Path]::Combine("..", "configurations", "docker-compose.services.yml")
$dockerComposeServicesOverridePath = [IO.Path]::Combine("..", "configurations", "docker-compose.services.$environmentName.yml")

docker-compose -f $dockerComposePath -f $dockerComposeServicesPath -f $dockerComposeServicesOverridePath `
               -p "equestria" down