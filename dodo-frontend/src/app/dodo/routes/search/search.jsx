import React, { Fragment, useEffect } from 'react'
import PropTypes from 'prop-types'
import { ListContainer, List } from 'app/components/events'
import { SiteMap } from 'app/components'
import { Filter } from './filter'

const mockParams = {
	latlong: "79+47",
	distance: 10000,
}

export const Search = ({ searchResults = [], params = mockParams, getSearchResults }) => {

	useEffect(() => {
		getSearchResults(mockParams)
	}, [params])


	return (
		<Fragment>
			<SiteMap/>
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
}