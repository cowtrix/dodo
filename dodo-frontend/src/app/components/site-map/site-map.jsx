import React from "react"
import PropTypes from "prop-types"
import { LeafletMap } from "app/components/leaflet-map"

export const SiteMap = (
	{
		sites = [],
		center,
		zoom,
		className,
		centerMap,
		setCenterMap,
		getSearchResults,
		searchParams,
		setSearchParams,
		display = true,
		selectedResourceTypes,
	}) =>
	display ?
		<LeafletMap
			centerMap={centerMap}
			setCenterMap={setCenterMap}
			sites={sites}
			center={center}
			zoom={zoom}
			className={className}
			getSearchResults={getSearchResults}
			searchParams={searchParams}
			setSearchParams={setSearchParams}
			selectedResourceTypes={selectedResourceTypes}
		/> :
		null

SiteMap.propTypes = {
	sites: PropTypes.array,
	center: PropTypes.array,
	zoom: PropTypes.number,
	className: PropTypes.string,
	setLocation: PropTypes.func,
	selectedResourceTypes:PropTypes.array,
}
