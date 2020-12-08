import React from 'react'
import PropTypes from 'prop-types'
import { Icon, Button } from 'app/components'
import styles from './search-by-location.module.scss'


const SEARCH_BY = "Search in my location"

const getSearchByCurrentLocation = (getSearchResults, searchParams, setCenterMap) => {
	navigator.geolocation.getCurrentPosition(position => {
		const loc = position.coords
		const latlong = loc.latitude + "+" + loc.longitude
		getSearchResults({ distance: "250", latlong }, setCenterMap)
	})
}

export const SearchByLocation = ({ getSearchResults, searchParams, setCenterMap }) =>
	<Button
		variant="link"
		className={styles.searchByLocation}
		onClick={() => getSearchByCurrentLocation(getSearchResults, searchParams, setCenterMap)}
	>
		{SEARCH_BY} <Icon icon="crosshairs"/>
	</Button>

SearchByLocation.propTypes = {
	getSearchResults: PropTypes.func,
	searchParams: PropTypes.object,
	setCenterMap: PropTypes.func,
}
