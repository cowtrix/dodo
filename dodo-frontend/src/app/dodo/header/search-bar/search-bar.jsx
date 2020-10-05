import React from "react"
import PropTypes from "prop-types"
import AsyncSelect from "react-select/async"
import { withRouter } from 'react-router-dom'


import styles from "./search-bar.module.scss"
import { useDebouncedCallback } from "use-debounce"
import { RSMaxInput } from "app/components/forms"

const placeholder = "Search location..."

export const SearchBar = withRouter(
	({
		searchString,
		search,
		history,
		setCenterMap,
	}) => {
		const searchDebounce = useDebouncedCallback(
			value => {
				if (value.length) {
					search({ search: value }, setCenterMap)
					value.length && history.push("/")
				}
			}, 500
		)

		return <AsyncSelect
			className={styles.searchBar}
			placeholder={placeholder}
			onInputChange={searchDebounce.callback}
			menuIsOpen={false}
			components={{ IndicatorsContainer: () => null, Input: RSMaxInput }}
		/>
	})

SearchBar.propTypes = {
	searchString: PropTypes.string,
	setCenterMap: PropTypes.func,
	setSearch: PropTypes.func,
	searchResults: PropTypes.array
}
