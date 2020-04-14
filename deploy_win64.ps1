git pull
# Kill running processes
$p = Get-Process -Name "DodoServer"
Stop-Process -InputObject $p
Get-Process | Where-Object {$_.HasExited}

$p = Get-Process -Name "node"
Stop-Process -InputObject $p
Get-Process | Where-Object {$_.HasExited}

# test - TODO
# dotnet test --project test\Dodo.UnitTests\Dodo.UnitTests.csproj
# publish
dotnet publish --force src\DodoServer\DodoServer.csproj -o ..\build -c Debug

# run
cd ..\build\
Start-Process -FilePath DodoServer.exe

