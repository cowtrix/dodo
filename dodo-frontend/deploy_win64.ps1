# assumes npm installed
# install dependencies
npm install @fortawesome/fontawesome-svg-core
npm install @types/googlemaps
npm install @types/markerclustererplus
npm install typescript@>=2.8.0

$p = Get-Process -Name "node"
if($p)
{
	Stop-Process -InputObject $p
	Get-Process | Where-Object {$_.HasExited}
}

#build and run
npm run-script build
npm install -g serve
serve build -l tcp://www.dodo.ovh:80