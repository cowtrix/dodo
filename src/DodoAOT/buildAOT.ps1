param([Parameter(Mandatory)][string] $configuration, [string] $solutionDir)

Write-Host "Configuration was $($configuration)"

if($configuration -ne "Debug")
{
	Write-Host "Skipping AOT compilation due to $configuration configuration"
	return
}

dotnet run --no-build /viewmodels:"$($solutionDir)src\DodoCMS\ViewModels" /views:"$($solutionDir)src\DodoServer\Views"
