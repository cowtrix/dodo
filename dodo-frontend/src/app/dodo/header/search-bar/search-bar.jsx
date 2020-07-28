import React from "react"
import PropTypes from "prop-types"
import AsyncSelect from "react-select/async"
import { withRouter } from 'react-router-dom'


import styles from "./search-bar.module.scss"

const placeholder = "Search location..."

export const SearchBar = withRouter((
	{
		searchString,
		search,
		history,
		setCenterMap,
	}) =>
	<AsyncSelect
		className={styles.searchBar}
		placeholder={placeholder}
		onInputChange={value => {
			if(value.length) {
				search({ search: value }, setCenterMap)
				value.length && history.push("/")
			}
		}}
		menuIsOpen={false}
		components={{ IndicatorsContainer: () => null }}
	/>)


SearchBar.propTypes = {
	searchString: PropTypes.string,
	setCenterMap: PropTypes.func,
	setSearch: PropTypes.func,
	searchResults: PropTypes.array
}
