import React from "react"
import PropTypes from "prop-types"
import { Marker, Popup } from "react-leaflet"
import leaflet from "leaflet"
import GreenMarker from "static/xr-pin-shadowed-green.svg"

const getLocationInfo = marker => {
	const { locationData = {} } = marker
	const { address = "", place = "", postcode = "" } = locationData

	return address ? `${address}, ${place}, ${postcode}` : ""
}

const MarkerIcon = leaflet.icon({
	iconUrl: GreenMarker,
	iconSize: [50, 40],
	iconAnchor: [25, 40],
	className: "xrMarker"
})

export const Markers = ({ markers, height = "400px" }) =>
	markers.map(marker => (
		<Marker
			icon={MarkerIcon}
			key={`${marker.latitude}_${marker.longitude}`}
			position={[marker.latitude, marker.longitude]}
		>
			{marker.locationData && <Popup>{getLocationInfo(marker)}</Popup>}
		</Marker>
	))

Markers.propTypes = {
	markers: PropTypes.array,
	height: PropTypes.string
}
