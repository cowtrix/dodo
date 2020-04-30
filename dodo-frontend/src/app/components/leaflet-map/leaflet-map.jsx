import React from "react"
import PropTypes from "prop-types"
import "leaflet/dist/leaflet.css"
import { Map } from "react-leaflet"

import { TitleLayers } from "./title-layers"
import { Markers } from "./markers"

import styles from "./leaflet-map.module.scss"

const getDefaultCenter = (sites, defaultLocation) => {
	if (sites && sites.length) {
		return [sites[0].location.latitude, sites[0].location.longitude]
	}
	return defaultLocation.length ? defaultLocation : [51.5074, 0.1278]
}

export const LeafletMap = ({
	sites,
	defaultLocation = [51.5074, 0.1278],
	zoom = 9
}) => {
	return (
		<Map
			className={styles.map}
			center={getDefaultCenter(sites, defaultLocation)}
			zoom={zoom}
		>
			<TitleLayers />
			<Markers markers={sites} />
		</Map>
	)
}

LeafletMap.propTypes = {
	markers: PropTypes.array,
	defaultLocation: PropTypes.array,
	zoom: PropTypes.number
}
