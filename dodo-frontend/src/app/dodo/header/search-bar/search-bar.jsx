import React from "react"
import PropTypes from "prop-types"
import AsyncSelect from "react-select/async"
import { Redirect, withRouter } from 'react-router-dom'


import styles from "./search-bar.module.scss"

const placeholder = "Search location..."

export const SearchBar = withRouter((
	{
		searchString,
		search,
		history,
	}) =>
	<AsyncSelect
		className={styles.searchBar}
		placeholder={placeholder}
		onInputChange={value => {
			search({ search: value })
			history.push("/")
		}}
		menuIsOpen={false}
	/>)


SearchBar.propTypes = {
	searchString: PropTypes.string,
	setSearch: PropTypes.func,
	searchResults: PropTypes.array
}
