import React, { Fragment, useEffect, useState } from "react"
import PropTypes from "prop-types"
import { ListContainer, List } from "app/components/events"
import { SiteMap, Loader } from "app/components"
import { Filter } from "./filter"

export const Search = ({
	searchResults = [],
	latlong,
	distance,
	getSearchResults,
	isFetchingSearch,
	searchSetCurrentLocation
}) => {
	useEffect(() => {
		searchSetCurrentLocation()
	}, [])

	useEffect(() => {
		if (latlong !== "") {
			getSearchResults(distance, latlong)
		}
	}, [latlong])

	return (
		<Fragment>
			<SiteMap sites={searchResults} />
			<ListContainer
				content={
					<Fragment>
						<Loader display={isFetchingSearch} />
						<Filter />
						<List events={searchResults} />
					</Fragment>
				}
			/>
		</Fragment>
	)
}

Search.propTypes = {
	isFetchingSearch: PropTypes.bool,
	getSearchResults: PropTypes.func,
	params: PropTypes.object,
	searchResults: PropTypes.array,
	searchSetCurrentLocation: PropTypes.func,
	setLocation: PropTypes.func
}
