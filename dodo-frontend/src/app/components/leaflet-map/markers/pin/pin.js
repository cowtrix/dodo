import leaflet from 'leaflet'
import { markers } from 'app/constants'

export const pin = (type) =>
	leaflet.icon({
		iconUrl: markers[type] ? markers[type] : markers.rebellion,
		iconSize: [50, 40],
		iconAnchor: [25, 40],
		className: "xrMarker"
	})
