import React from 'react'
import PropTypes from 'prop-types'
import { dateFormatted, timeFormatted } from './services'

const singleDayEvent = (startDate, endDate, options) => dateFormatted(startDate, options) === dateFormatted(endDate, options)

export const Dates = ({ startDate, endDate}) => {

	const dateOptions = { day: "2-digit", month: "long", year: "numeric"}
	const dateAndTimeOptions = { day: "2-digit", month: "long", year: "numeric", hour: '2-digit', minute: '2-digit'}
	const timeOptions = { hour: '2-digit', minute: '2-digit'}

	return (
		startDate ?
			<h3>
			{singleDayEvent(startDate, endDate, dateOptions) ?
				dateFormatted(startDate, dateAndTimeOptions) + ' - ' + timeFormatted(endDate, timeOptions)  :
				`${dateFormatted(startDate, dateOptions) + ' - ' + dateFormatted(endDate, dateOptions)}`}
		</h3> :
			null
	)
}


Date.propTypes = {
	startDate: PropTypes.string,
	endDate: PropTypes.string,
}