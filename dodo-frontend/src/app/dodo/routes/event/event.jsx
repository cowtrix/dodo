import React, { useEffect } from "react"
import PropTypes from "prop-types"

import { ContentPage, RebellionDetail, RebellionEvents } from "app/components"

export const Event = ({ match, getEvent, event }) => {
	console.log(match)
	const { eventId, eventType } = match.params

	useEffect(() => {
		getEvent(eventType, eventId)
	}, [])

	if (!event) {
		return <div>Loading</div>
	}

	return (
		<ContentPage sideBar={<RebellionEvents events={[]} />}>
			<RebellionDetail rebellion={event} />
		</ContentPage>
	)
}

Event.propTypes = {
	match: PropTypes.object.isRequired,
	getRebellion: PropTypes.func,
	rebellion: PropTypes.object
}
