import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { Container, List } from "app/components/events"
import { SiteMap, Loader } from "app/components"
import { Filter } from "./filter"

export const Search = ({
	searchResults,
	searchParams,
	getSearchResults,
	isFetchingSearch
}) => {
	useEffect(() => {
		if (searchParams.latlong !== "") {
			getSearchResults(searchParams)
		}
	}, [searchParams.distance, searchParams.latlong])

	return (
		<Fragment>
			<SiteMap sites={searchResults} />
			<Container
				content={
					<Fragment>
						<Loader display={!searchParams.latlong.length || isFetchingSearch} />
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
