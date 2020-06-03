import React from "react"
import PropTypes from "prop-types"
import AsyncSelect from "react-select/async"
import { CustomSelect } from "./custom-select"

import styles from "./search-bar.module.scss"

const placeholder = "Search location..."

export const SearchBar = (
	{
		searchString,
		search,
		searchResults,
		history,
		searchParams,
	}) =>
	<AsyncSelect
		defaultOptions={searchResults.map(result => ({
			label: result.name,
			value: result.guid,
			data: result
		}))}
		loadOptions={(search, cb) => {
			cb(
				searchResults.map(result => ({
					label: result.name,
					value: result.guid,
					data: result
				}))
			)
		}}
		className={styles.searchBar}
		placeholder={placeholder}
		onInputChange={value => search({ ...searchParams, search: value })}
		formatOptionLabel={CustomSelect}
	/>


SearchBar.propTypes = {
	searchString: PropTypes.string,
	setSearch: PropTypes.func,
	searchResults: PropTypes.array
}
