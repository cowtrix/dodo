import React from 'react'
import PropTypes from 'prop-types'
import { dateFormatted } from './services'

export const Dates = ({ startDate, endDate}) => {

	const dateOptions = { day: "2-digit", month: "long", year: "numeric"}

	return (
		<h3>
			{dateFormatted(startDate, dateOptions)} - {dateFormatted(endDate, dateOptions)}
		</h3>
	)
}


Date.propTypes = {
	startDate: PropTypes.string,
	endDate: PropTypes.string,
}