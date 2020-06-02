import React, { useEffect, useState } from "react"
import PropTypes from "prop-types"
import "leaflet/dist/leaflet.css"
import { Map } from "react-leaflet"

import { TitleLayers } from "./title-layers"
import { Markers } from "./markers"

import styles from "./leaflet-map.module.scss"

const getDefaultCenter = (sites, location) => {
	if (sites && sites.length) {
		return [sites[0].location.latitude, sites[0].location.longitude]
	}
	return location && location.length ? location : [51.5074, 0.1278]
}

export const LeafletMap = ({
	sites,
	center,
	zoom = 9,
	className,
	centerMap,
	setCenterMap,
}) => {

 const [mapCenter, setMapCenter] = useState(getDefaultCenter(sites, center))

	useEffect(() => {
		setMapCenter(getDefaultCenter(sites, center))
		setCenterMap(false)
	}, [centerMap, center])


	return (
		<Map
			className={`${styles.map} ${className}`}
			center={mapCenter}
			zoom={zoom}
			viewport={mapCenter}
		>
			<TitleLayers/>
			<Markers markers={sites} />
		</Map>
	)
}

LeafletMap.propTypes = {
	markers: PropTypes.array,
	location: PropTypes.array,
	zoom: PropTypes.number
}
