import React, { Fragment, useState } from "react"
import PropTypes from "prop-types"
import { Container } from "app/components/resources"
import { SiteMap, Loader, CenterMap, List, Video } from "app/components"
import { Filter } from "./filter"

import styles from './search.module.scss'
import { SearchByLocation } from './search-by-location'


export const Search = (
	{
		homeVideo,
		centerMap,
		setCenterMap,
		searchResults,
		searchParams,
		getSearchResults,
		isFetchingSearch,
		resourceTypes,
		setSearchParams,
	}) => {

	useState(() => {
		getSearchResults({}, setCenterMap)
	}, [])

	return (
		<Fragment>
			<SiteMap
				centerMap={centerMap}
				setCenterMap={setCenterMap}
				sites={searchResults}
				getSearchResults={getSearchResults}
				searchParams={searchParams}
				setSearchParams={setSearchParams}
				selectedResourceTypes={searchParams.types}
			/>
			<Container
				content={
					<Fragment>
						<Loader display={isFetchingSearch}/>
						<div className={styles.searchHeader}>
							<Filter/>
							<SearchByLocation
								getSearchResults={getSearchResults}
								searchParams={searchParams}
								setCenterMap={setCenterMap}
							/>
						</div>
						<List resources={searchResults} resourceTypes={resourceTypes} isMasterList/>
					</Fragment>
				}
			/>
		</Fragment>
	)
}

Search.propTypes = {
	homeVideo: PropTypes.string,
	isFetchingSearch: PropTypes.bool,
	getSearchResults: PropTypes.func,
	params: PropTypes.object,
	searchResults: PropTypes.array,
	searchSetCurrentLocation: PropTypes.func,
	setSearchParams: PropTypes.func,
	centerMap: PropTypes.oneOfType([
		PropTypes.array, PropTypes.bool,
	]),
}
