# Kill running processes
$p = Get-Process -Name "DodoServer"
Stop-Process -InputObject $p
Get-Process | Where-Object {$_.HasExited}
# test - TODO
# dotnet test --project test\Dodo.UnitTests\Dodo.UnitTests.csproj
# publish
dotnet publish --force src\DodoServer\DodoServer.csproj -o ..\build -c Debug
Start-Sleep -Seconds 2
# run
Start-Process -FilePath ..\build\DodoServer.exe