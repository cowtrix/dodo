import React, { Fragment, useEffect, useState } from "react"
import PropTypes from "prop-types"
import { ListContainer, List } from "app/components/events"
import { SiteMap } from "app/components"
import { Filter } from "./filter"

export const Search = ({
	searchResults = [],
	latlong,
	distance,
	getSearchResults,
	initialSearchResults = []
}) => {
	const [defaultMapCenter, setdefaultMapCenter] = useState([])

	const success = position => {
		const loc = position.coords
		const latlong = loc.latitude + "+" + loc.longitude
		setdefaultMapCenter([loc.latitude, loc.longitude])
		getSearchResults(distance, latlong)
	}

	useEffect(() => {
		navigator.geolocation.getCurrentPosition(success)
	}, [distance, getSearchResults, initialSearchResults])

	return (
		<Fragment>
			<SiteMap defaultLocation={defaultMapCenter} sites={searchResults} />
			<ListContainer
				content={
					<Fragment>
						<Filter />
						<List events={searchResults} />
					</Fragment>
				}
			/>
		</Fragment>
	)
}

Search.propTypes = {
	getSearchResults: PropTypes.func,
	params: PropTypes.object,
	searchResults: PropTypes.array,
	setLocation: PropTypes.func
}
