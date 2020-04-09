import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./selector.module.scss"
import { formatEvents } from "./services"

export const Selector = ({
	eventTypes,
	searchFilterEvents,
	eventsFiltered
}) => (
	<Select
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

Selector.propTypes = {
	eventTypes: PropTypes.array
}
