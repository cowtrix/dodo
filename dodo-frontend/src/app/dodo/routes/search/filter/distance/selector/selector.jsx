import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./selector.module.scss"
import { formatEvents } from "./services"

const list = [1, 10, 100, 1000, 10000]

export const Selector = ({
	latlong,
	distance,
	updateDistance,
	placeholder
}) => (
	<Select
		placeholder={placeholder}
		defaultValue={distance}
		options={formatEvents(list)}
		className={styles.selector}
		onChange={value => updateDistance(value.value, latlong)}
	/>
)

Selector.propTypes = {
	eventTypes: PropTypes.array,
	eventsFiltered: PropTypes.array,
	placeholder: PropTypes.string,
	searchFilterEvents: PropTypes.func
}
