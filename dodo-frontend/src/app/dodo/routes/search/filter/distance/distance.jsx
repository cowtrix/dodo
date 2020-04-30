import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./distance.module.scss"
import { formatEvents } from "./services"

const list = [1, 10, 100, 1000, 10000]
const placeholder = "Distance..."

export const Distance = ({ latlong, distance, updateDistance }) => (
	<Select
		placeholder={placeholder}
		defaultValue={distance}
		options={formatEvents(list)}
		className={styles.selector}
		onChange={value => updateDistance(value.value, latlong)}
	/>
)

Distance.propTypes = {
	eventTypes: PropTypes.array,
	eventsFiltered: PropTypes.array,
	placeholder: PropTypes.string,
	searchFilterEvents: PropTypes.func
}
