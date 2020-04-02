import React, { useEffect, useState } from "react"
import {
	fetchRebellion,
	fetchRebellionEvents
} from "app/domain/services/rebellion"
import PropTypes from "prop-types"

import { ContentPage, RebellionDetail, RebellionEvents } from "app/components"

export const Rebellion = ({ match }) => {
	const { rebellionId } = match.params
	const [rebellion, setRebellion] = useState()
	const [events, setEvents] = useState()

	useEffect(() => {
		const load = async () => {
			setRebellion(await fetchRebellion(rebellionId))
			setEvents(await fetchRebellionEvents(rebellionId))
		}
		load()
	}, [rebellionId])

	if (!rebellion) {
		return <div>Loading</div>
	}

	return (
		<ContentPage sideBar={<RebellionEvents events={events} />}>
			<RebellionDetail rebellion={rebellion} />
		</ContentPage>
	)
}

Rebellion.propTypes = {
	match: PropTypes.object.isRequired
}
