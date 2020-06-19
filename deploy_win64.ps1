git pull

# test
if($test)
{
	$testResult = dotnet test
	Write-Output "Running tests"
	if($testResult.contains("Failed: "))
	{
		Write-Error $testResult
		return -1
	}
	Write-Output "All tests passed"
}

# Kill running processes
$p = Get-Process -Name "DodoServer"
if($p)
{
	Stop-Process -InputObject $p
	Get-Process | Where-Object {$_.HasExited}
}

# publish
dotnet publish --force src\DodoServer\DodoServer.csproj -o ..\build -c Release

# run
cd ..\build\
Start-Process -FilePath DodoServer.exe

