import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./events.module.scss"
import { formatEvents } from "./services"

const placeholder = "Event types..."

export const Events = ({ eventTypes, searchFilterEvents, eventsFiltered }) => (
	<Select
		placeholder={placeholder}
		isMulti
		defaultValue={formatEvents(eventsFiltered)}
		options={formatEvents(eventTypes)}
		className={styles.selector}
		onChange={value =>
			value
				? searchFilterEvents(value.map(event => event.value))
				: searchFilterEvents([])
		}
	/>
)

Events.propTypes = {
	eventTypes: PropTypes.array,
	eventsFiltered: PropTypes.array,
	placeholder: PropTypes.string,
	searchFilterEvents: PropTypes.func
}
