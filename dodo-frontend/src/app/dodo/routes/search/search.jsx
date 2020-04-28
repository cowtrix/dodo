import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { ListContainer, List } from "app/components/events"
import { SiteMap } from "app/components"
import { Filter } from "./filter"

export const Search = ({ searchResults = [], params, getSearchResults }) => {
	const location = geolocationPositionInstance.coords
	console.log(location)

	useEffect(() => {
		getSearchResults(params)
	}, [params, getSearchResults])

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
	searchResults: PropTypes.array
}
