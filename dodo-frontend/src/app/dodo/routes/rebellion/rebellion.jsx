import React, { useEffect, useState } from "react"
import PropTypes from "prop-types"

import { useFetch } from "app/domain/services/useFetch"
import {
	fetchRebellion,
	fetchRebellionEvents
} from "app/domain/services/rebellion"

import { ContentPage, RebellionDetail, RebellionEvents } from "app/components"

export const Rebellion = ({ match }) => {
	const { rebellionId } = match.params
	const rebellion = useFetch(fetchRebellion, rebellionId)
	const events = useFetch(fetchRebellionEvents, rebellionId)

	if (!rebellion) {
		return <div>Loading</div>
	}

	return (
		<ContentPage sideBar={<RebellionEvents events={events || []} />}>
			<RebellionDetail rebellion={rebellion} />
		</ContentPage>
	)
}

Rebellion.propTypes = {
	match: PropTypes.object.isRequired
}
