import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"

import styles from "./search-bar.module.scss"
import { api } from "app/domain/services"

const placeholder = "Search location..."

const mapQuestApi = (key, location) =>
	`${"http://open.mapquestapi.com/geocoding/v1/address?key=" +
		key +
		"&" +
		"location=" +
		location}`

const questKey = "PutjB2T72jl2o910ArMCo4CGsOgvjPp4"

export const SearchBar = ({ searchValues }) => (
	<Select
		className={styles.searchBar}
		placeholder={placeholder}
		onChange={value => {
			console.log(value.target.value)
			api(mapQuestApi(questKey, value.target.value))
		}}
	/>
)

SearchBar.propTypes = {
	searchValues: PropTypes.array
}
