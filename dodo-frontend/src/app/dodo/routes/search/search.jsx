import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { Container } from "app/components/resources"
import { SiteMap, Loader, CenterMap, List } from "app/components"
import { Filter } from "./filter"

import styles from './search.module.scss'


export const Search = ({
	centerMap,
	setCenterMap,
	searchResults,
	searchParams,
	getSearchResults,
	isFetchingSearch
}) => {
	useEffect(() => {
		if (searchParams.latlong !== "") {
			getSearchResults(searchParams)
		}
	}, [searchParams.distance, searchParams.latlong, searchParams.search])

	return (
		<Fragment>
			<SiteMap
				centerMap={centerMap}
				setCenterMap={setCenterMap}
				sites={searchResults}
			/>
			<Container
				content={
					<Fragment>
						<Loader display={isFetchingSearch} />
						<div className={styles.searchHeader}>
							<Filter />
							<CenterMap setCenterMap={setCenterMap}/>
						</div>
						<List resources={searchResults} />
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
