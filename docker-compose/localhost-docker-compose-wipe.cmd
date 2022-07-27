set batchPath=%~dp0
cd ./scripts
pwsh -file "./docker-compose-wipe.ps1" -environmentName "localhost" -environmentVariablesPath "../configurations/localhost.env"
cd %batchPath%
PAUSE