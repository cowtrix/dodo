import React from "react"
import { LeafletMap } from "app/components/leaflet-map"

export const SiteMap = ({ sites = [], height = "220px" }) => (
	<LeafletMap markers={sites.map(site => site.location)} />
)
//export const SiteMap = React.memo(({ sites = [], height = "220px" }) => {
//	const markers = sites.map(site => site.location)
//	return <MapboxMap markers={markers} height={height} />
//})
