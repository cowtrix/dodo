# assumes npm installed
# install dependencies
npm install @fortawesome/fontawesome-svg-core
npm install @types/googlemaps
npm install @types/markerclustererplus
npm install typescript@>=2.8.0

#build and run
npm run-script build
npm install -g serve
serve build -l tcp://www.dodo.ovh:80