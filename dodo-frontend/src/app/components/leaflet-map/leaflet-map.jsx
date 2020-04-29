import React from "react"
import leaflet from "leaflet"
import "leaflet/dist/leaflet.css"
import { Map, Marker, Popup, TileLayer } from "react-leaflet"

import GreenMarker from "static/xr-pin-shadowed-green.svg"

const TILE_LAYERS = [
	{
		attribution:
			'© <a href="http://osm.org/copyright">OpenStreetMap</a> contributors',
		url: "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
	},
	{
		url:
			"https://maps.rebellion.global/styles/xr_places_non-latin/{z}/{x}/{y}@2x.png"
	}
]

const getDefaultCenter = markers => {
	if (markers && markers.length) {
		return [markers[0].latitude, markers[0].longitude]
	}
	return [51.5074, 0.1278]
}

const MarkerIcon = leaflet.icon({
	iconUrl: GreenMarker,
	iconSize: [50, 40],
	iconAnchor: [25, 40],
	className: "xrMarker"
})

export const LeafletMap = ({ markers }) => {
	return (
		<Map
			center={getDefaultCenter(markers)}
			zoom={9}
			style={{ height: "220px" }}
		>
			{TILE_LAYERS.map(tileLayer => (
				<TileLayer key={tileLayer.url} {...tileLayer} />
			))}
			{markers.map(marker => (
				<Marker
					icon={MarkerIcon}
					key={`${marker.latitude}_${marker.longitude}`}
					position={[marker.latitude, marker.longitude]}
				>
					<Popup>
						A pretty CSS3 popup. <br /> Easily customizable.
					</Popup>
				</Marker>
			))}
		</Map>
	)
}
