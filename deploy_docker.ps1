$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path
cd $ScriptDir

docker build . -t dodo:latest
docker pull mongo:latest
docker compose up