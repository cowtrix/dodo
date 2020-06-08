$p = Get-Process -Name "node"
if($p)
{
	Stop-Process -InputObject $p
	Get-Process | Where-Object {$_.HasExited}
}

#build and run
npm i
npm run-script build