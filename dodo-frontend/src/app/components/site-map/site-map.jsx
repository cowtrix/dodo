import React from "react"
import PropTypes from "prop-types"
import { LeafletMap } from "app/components/leaflet-map"

export const SiteMap = ({ sites = [], center, zoom, className, centerMap, setCenterMap }) => (
	<LeafletMap
		centerMap={centerMap}
		setCenterMap={setCenterMap}
		sites={sites}
		center={center}
		zoom={zoom}
		className={className}
	/>
)

SiteMap.propTypes = {
	sites: PropTypes.array,
	center: PropTypes.array,
	zoom: PropTypes.number,
	className: PropTypes.string,
}
