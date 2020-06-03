import React, { Fragment, useEffect } from "react"
import PropTypes from "prop-types"
import { Container } from "app/components/events"
import { SiteMap, Loader, DateLayout, PageTitle } from "app/components"
import { EventContent } from './event-content'

export const Event = ({ match, getEvent, event, isLoading, centerMap, setCenterMap }) => {
	const { eventId, eventType } = match.params

	useEffect(() => {
		getEvent(eventType, eventId)
	}, [match])

	const { location } = event
	const defaultLocation = event.location
		? [location.latitude, location.longitude]
		: []

	return (
		<Fragment>
			<SiteMap
				centerMap={centerMap}
				setCenterMap={setCenterMap}
				center={defaultLocation}
				sites={event.sites && [...event.sites, ...event.workingGroups]}
			/>
			<button onClick={() => setCenterMap(true)} >SET MAP</button>
			<Container
				content={
					<Fragment>
						<Loader display={isLoading} />
						{event.metadata && !isLoading && (
							<EventContent event={event} setCenterMap={setCenterMap}/>
						)}
					</Fragment>
				}
			/>
		</Fragment>
	)
}

Event.propTypes = {
	match: PropTypes.object.isRequired,
	getEvent: PropTypes.func,
	event: PropTypes.object
}
