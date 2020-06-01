import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./events.module.scss"
import { formatEvents } from "./services"

const placeholder = "Event types..."

export const Events = ({ eventTypes, searchParams, searchByEvent }) => (
	<Select
		placeholder={placeholder}
		isMulti
		defaultValue={formatEvents(eventTypes)}
		options={formatEvents(eventTypes)}
		className={styles.selector}
		onChange={value => searchByEvent({ ...searchParams, events: value.map(event => event.value) })}
	/>
)

Events.propTypes = {
	eventTypes: PropTypes.array,
	eventsFiltered: PropTypes.array,
	placeholder: PropTypes.string,
	searchFilterEvents: PropTypes.func
}
