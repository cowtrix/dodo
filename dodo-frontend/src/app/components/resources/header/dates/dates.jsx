import React from 'react'
import PropTypes from 'prop-types'
import { dateFormatted } from './services'

export const Dates = ({ startDate, endDate}) => {

	const dateOptions = { day: "2-digit", month: "long", year: "numeric"}

	return (
		startDate ?
			<h3>
			{startDate === endDate ?
				dateFormatted(startDate, dateOptions) :
				`${dateFormatted(startDate, dateOptions) + ' - ' + dateFormatted(endDate, dateOptions)}`}
		</h3> :
			null
	)
}


Date.propTypes = {
	startDate: PropTypes.string,
	endDate: PropTypes.string,
}