set batchPath=%~dp0
cd ./scripts
pwsh -file "./docker-compose-up.ps1" -environmentName "localhost" -environmentVariablesPath "../configurations/localhost.env" -progress -build
cd %batchPath%
PAUSE