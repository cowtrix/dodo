import React from "react"
import PropTypes from "prop-types"
import "leaflet/dist/leaflet.css"
import { Map } from "react-leaflet"

import { TitleLayers } from "./title-layers"
import { Markers } from "./markers"

const getDefaultCenter = (sites, defaultLocation) => {
	if (sites && sites.length) {
		return [sites[0].location.latitude, sites[0].location.longitude]
	}
	return defaultLocation.length ? defaultLocation : [51.5074, 0.1278]
}

export const LeafletMap = ({
	sites,
	height = "400px",
	defaultLocation = [51.5074, 0.1278],
	zoom = 9
}) => {
	return (
		<Map
			center={getDefaultCenter(sites, defaultLocation)}
			zoom={zoom}
			style={{ height: height }}
		>
			<TitleLayers />
			<Markers markers={sites} height={height} />
		</Map>
	)
}

LeafletMap.propTypes = {
	markers: PropTypes.array,
	height: PropTypes.string,
	defaultLocation: PropTypes.array,
	zoom: PropTypes.number
}
