import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { ListContainer, List } from "app/components/events"
import { SiteMap } from "app/components"
import { Filter } from "./filter"

export const Search = ({
	searchResults = [],
	latlong,
	distance,
	getSearchResults,
	setLocation,
	params
}) => {
	const success = position => {
		const loc = position.coords
		const latlong = loc.latitude + "+" + loc.longitude
		getSearchResults(distance, latlong)
	}

	useEffect(() => {
		navigator.geolocation.getCurrentPosition(success)
		latlong && distance && getSearchResults(distance, latlong)
	}, [latlong, distance, getSearchResults])

	return (
		<Fragment>
			<SiteMap />
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
