import React from "react"
import PropTypes from "prop-types"
import { Marker } from "react-leaflet"
import { Popup } from "./popup"
import { pin } from './pin'


export const Markers = ({ markers, userInitiated }) =>
	markers.map(marker =>
		marker.location ? (
			<Marker
				icon={pin(marker.metadata.type)}
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
