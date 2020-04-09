import React from "react"
import PropTypes from "prop-types"
import { Dialog as DialogWrapper } from "app/components"
import { Selector } from "../selector"

export const Dialog = ({
	active,
	closeDialog,
	eventTypes,
	searchFilterEvents,
	eventsFiltered
}) => (
	<DialogWrapper
		active={active}
		title="Events"
		content={
			<Selector
				eventTypes={eventTypes}
				searchFilterEvents={searchFilterEvents}
				eventsFiltered={eventsFiltered}
			/>
		}
		close={closeDialog}
		update={closeDialog}
	/>
)

Dialog.propTypes = {
	active: PropTypes.bool,
	closeDialog: PropTypes.func
}
