import React from "react"
import PropTypes from "prop-types"
import { LeafletMap } from "app/components/leaflet-map"

export const SiteMap = ({ sites = [], defaultLocation, zoom }) => (
	<LeafletMap sites={sites} defaultLocation={defaultLocation} zoom={zoom} />
)

SiteMap.propTypes = {
	sites: PropTypes.array,
	defaultLocation: PropTypes.object,
	zoom: PropTypes.number
}
