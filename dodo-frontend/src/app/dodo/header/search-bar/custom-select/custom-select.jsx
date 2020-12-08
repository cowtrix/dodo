import React from "react"
import PropTypes from "prop-types"
import { withRouter } from "react-router-dom"

const goToEvent = (history, data) => () => {
	const event = data
	const eventString = "/" + event.metadata.type + "/" + event.slug
	history.push(eventString)
}

export const CustomSelect = withRouter(({ history, data }) => (
	<div onClick={goToEvent(history, data)} onKeyUp={goToEvent(history, data)}>
		{data.name}
	</div>
))

CustomSelect.propTypes = {
	data: PropTypes.object
}
