import React from "react"
import PropTypes from "prop-types"
import { LeafletMap } from "app/components/leaflet-map"

export const SiteMap = ({ sites = [], defaultLocation }) => (
	<LeafletMap sites={sites} defaultLocation={defaultLocation} />
)

SiteMap.propTypes = {
	sites: PropTypes.array,
	defaultLocation: PropTypes.object
}
