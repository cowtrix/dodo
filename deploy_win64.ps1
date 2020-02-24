# Kill running processes
$p = Get-Process -Name "DodoIdentity"
Stop-Process -InputObject $p
Get-Process | Where-Object {$_.HasExited}
$p = Get-Process -Name "DodoResources"
Stop-Process -InputObject $p
Get-Process | Where-Object {$_.HasExited}
# test - TODO
# dotnet test --project test\Dodo.UnitTests\Dodo.UnitTests.csproj
# publish
dotnet publish --force src\DodoIdentity\DodoIdentity.csproj -o ..\build\auth -c Debug
dotnet publish --force src\DodoResources\DodoResources.csproj -o ..\build\rsc -c Debug
Start-Sleep -Seconds 2
# run
Start-Process -FilePath ..\build\auth\DodoIdentity.exe
Start-Sleep -Seconds 2
Start-Process -FilePath ..\build\rsc\DodoResources.exe