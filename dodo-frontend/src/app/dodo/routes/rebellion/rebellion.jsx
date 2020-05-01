import React, { useEffect } from "react"
import PropTypes from "prop-types"

import { useFetch } from "app/domain/services/useFetch"
import {
	fetchRebellion,
	fetchRebellionEvents
} from "app/domain/services/rebellion"

import { ContentPage, RebellionDetail, RebellionEvents } from "app/components"

export const Rebellion = ({ match, getRebellion, rebellion }) => {
	const { rebellionId } = match.params

	useEffect(() => {
		getRebellion(rebellionId)
	}, [])

	if (!rebellion) {
		return <div>Loading</div>
	}

	return (
		<ContentPage sideBar={<RebellionEvents events={[]} />}>
			<RebellionDetail rebellion={rebellion} />
		</ContentPage>
	)
}

Rebellion.propTypes = {
	match: PropTypes.object.isRequired,
	getRebellion: PropTypes.func,
	rebellion: PropTypes.object
}
