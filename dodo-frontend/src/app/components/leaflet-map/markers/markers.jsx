import React from "react"
import PropTypes from "prop-types"
import { Marker } from "react-leaflet"
import leaflet from "leaflet"
import GreenMarker from "static/xr-pin-shadowed-green.svg"
import { Popup } from "./popup"

const MarkerIcon = leaflet.icon({
	iconUrl: GreenMarker,
	iconSize: [50, 40],
	iconAnchor: [25, 40],
	className: "xrMarker"
})

export const Markers = ({ markers, userInitiated }) =>
	markers.map(marker =>
		marker.location ? (
			<Marker
				icon={MarkerIcon}
				key={marker.guid}
				position={[marker.location.latitude, marker.location.longitude]}
			>
				{userInitiated ? <Popup site={marker} /> : null}
			</Marker>
		) : null
	)

Markers.propTypes = {
	markers: PropTypes.array
}
