import React from "react"
import PropTypes from "prop-types"
import { Button, Selector as SelectorWrapper } from "app/components"
import { Selector } from "./selector"

const eventsTitle = "Event Types..."

export const Distance = ({
	eventTypes,
	eventsFiltered,
	searchFilterEvents
}) => (
	<SelectorWrapper
		title={eventsTitle}
		content={
			<Selector
				placeholder={eventsTitle}
				eventTypes={eventTypes}
				eventsFiltered={eventsFiltered}
				searchFilterEvents={searchFilterEvents}
			/>
		}
	/>
)

Distance.propTypes = {
	eventTypes: PropTypes.array,
	eventsFiltered: PropTypes.array,
	searchFilterEvents: PropTypes.func
}
