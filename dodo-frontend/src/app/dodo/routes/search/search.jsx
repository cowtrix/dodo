import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { Container, List } from "app/components/events"
import { SiteMap, Loader } from "app/components"
import { Filter } from "./filter"

export const Search = ({
	searchResults = [],
	latlong,
	distance,
	search,
	getSearchResults,
	isFetchingSearch
}) => {
	useEffect(() => {
		if (latlong !== "") {
			getSearchResults(distance, latlong, search)
		}
	}, [latlong])

	return (
		<Fragment>
			<SiteMap sites={searchResults} />
			<Container
				content={
					<Fragment>
						<Loader display={latlong === "" || isFetchingSearch} />
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
