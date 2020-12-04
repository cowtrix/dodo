import React, { useEffect, useState } from "react"
import PropTypes from "prop-types"
import "leaflet/dist/leaflet.css"
import { Map, Marker } from "react-leaflet"

import { TitleLayers } from "./title-layers"
import { Markers, pin } from "./markers"

import styles from "./leaflet-map.module.scss"

const getDefaultCenter = (sites = [], location = []) => {
	const sitesWithLocation = sites.length && sites.filter(site => site.location)
	const firstSiteLocation = sitesWithLocation.length && [sitesWithLocation[0].location.latitude, sitesWithLocation[0].location.longitude]
	return firstSiteLocation ? firstSiteLocation : location.length ? location : [51.5074, 0.1278]
}

export const LeafletMap = (
	{
		sites = [],
		center = [],
		zoom = 9,
		className,
		centerMap,
		setCenterMap,
		getSearchResults = () => {},
		setSearchParams = () => {},
		selectedResourceTypes,
		resourceType
	}) => {

	const [mapCenter, setMapCenter] = useState(getDefaultCenter(sites, center))
	const [userInitiated, setUserInitiated] = useState(false)

	useEffect(() => {
		if (centerMap) {
			setMapCenter(getDefaultCenter(sites, center))
			setCenterMap(false)
		}

		setNewSearchParams(this);
	}, [centerMap, center, setCenterMap, sites])


	const setNewSearchParams = (e) => {
		if (!e || !e.target) {
			console.warn("target was undefined")
			return
		}
		const newSearchCenter = e.target.getCenter()
		const newSearchDistance = e.target.getZoom()
		const metersPerPx = (156543.03392 * Math.cos(newSearchCenter.lat * Math.PI / 180) / Math.pow(2, newSearchDistance)) / 2
		if (userInitiated) {
			getSearchResults({ distance: metersPerPx.toString(), latlong: newSearchCenter.lat + '+' + newSearchCenter.lng, types: selectedResourceTypes })
			setUserInitiated(false)
		}
		else {
			setSearchParams({ distance: metersPerPx.toString(), latlong: newSearchCenter.lat + '+' + newSearchCenter.lng, types: selectedResourceTypes })
		}
	}

	return (
		<Map
			className={`${styles.map} ${className}`}
			center={mapCenter}
			zoom={zoom}
			viewport={mapCenter}
			onZoomanim={() => setUserInitiated(true)}
			onMousedown={() => setUserInitiated(true)}
			onMoveend={setNewSearchParams}
		>
			<TitleLayers/>
			<Markers markers={sites} userInitiated={userInitiated}/>
			{!sites.length && center.length ?
				<Marker position={center} icon={pin(resourceType)}/> :
				null}
		</Map>
	)
}

LeafletMap.propTypes = {
	markers: PropTypes.array,
	location: PropTypes.array,
	zoom: PropTypes.number,
	setSearchParams: PropTypes.func,
	selectedResourceTypes:PropTypes.array,
}
