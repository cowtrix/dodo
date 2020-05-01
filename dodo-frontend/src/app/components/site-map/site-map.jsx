import React from "react"
import PropTypes from "prop-types"
import { LeafletMap } from "app/components/leaflet-map"

export const SiteMap = ({ sites = [], defaultLocation, zoom, className }) => (
	<LeafletMap
		sites={sites}
		defaultLocation={defaultLocation}
		zoom={zoom}
		className={className}
	/>
)

SiteMap.propTypes = {
	sites: PropTypes.array,
	defaultLocation: PropTypes.array,
	zoom: PropTypes.number
}
