param(
[String]$environmentName = "localhost",
[String]$environmentVariablesPath = [IO.Path]::Combine("..", "configurations", "localhost.env"),
[int]$updateImages = 1,
[switch]$progress,
[switch]$build,
[switch]$forceRecreate)

$commonScriptPath = [IO.Path]::Combine($PSScriptRoot, "common.ps1")
. $commonScriptPath -environmentName $environmentName -environmentVariablesPath $environmentVariablesPath

$dockerComposePath = [IO.Path]::Combine("..", "configurations", "docker-compose.yml")
$dockerComposeServicesPath = [IO.Path]::Combine("..", "configurations", "docker-compose.services.yml")
$dockerComposeServicesOverridePath = [IO.Path]::Combine("..", "configurations", "docker-compose.services.$environmentName.yml")

if ($updateImages -eq 1) {
  docker-compose -f $dockerComposePath -f $dockerComposeServicesPath -f $dockerComposeServicesOverridePath `
                 -p "equestria" pull (&{If($progress) {""} Else {"--quiet"}})
}

docker-compose -f $dockerComposePath -f $dockerComposeServicesPath -f $dockerComposeServicesOverridePath `
               -p "equestria" up -d (&{If($build) {"--build"} Else {""}}) (&{If($forceRecreate) {"--force-recreate"} Else {""}}) --remove-orphans
