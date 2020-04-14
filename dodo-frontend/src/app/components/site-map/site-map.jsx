import React from "react"
import { Map } from "app/components/index"

export const SiteMap = ({ sites = [], height = "220px" }) => {
	const markers = sites.map(site => site.Location)

	return (
		<Map
			isMarkerShown
			googleMapURL="https://maps.googleapis.com/maps/api/js?v=3.exp&libraries=geometry,drawing,places"
			loadingElement={<div style={{ height: "100%" }} />}
			containerElement={<div style={{ height }} />}
			mapElement={<div style={{ height: "100%" }} />}
			options={{ fullscreenControl: false, zoomControl: false }}
			markers={markers}
		/>
	)
}
//export const SiteMap = React.memo(({ sites = [], height = "220px" }) => {
//	const markers = sites.map(site => site.location)
//	return <MapboxMap markers={markers} height={height} />
//})
