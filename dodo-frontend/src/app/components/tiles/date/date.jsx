import React from "react"
import PropTypes from "prop-types"

import styles from "./date.module.scss"
import { getTileColor } from "./services"

export const Date = ({ startDate, endDate = null, backgroundColor }) => {
	const splitStartDate = `${startDate}`.split(" ")
	const style = backgroundColor
		? { backgroundColor }
		: { backgroundColor: getTileColor(startDate, endDate) }

	const splitEndDate = `${endDate}`.split(" ")

	const dayDate = endDate =>
		!endDate
			? splitStartDate[2]
			: endDate.getMonth() === startDate.getMonth()
			? splitStartDate[2] + "-" + splitEndDate[2]
			: splitStartDate[2] + "-" + splitStartDate[1]

	const monthDate = endDate =>
		!endDate || endDate.getMonth() === startDate.getMonth()
			? splitStartDate[1]
			: "-" + splitEndDate[2] + "-" + splitEndDate[1]

	return (
		<div className={styles.tile} style={style}>
			<div className={styles.dayDate}>{dayDate(endDate)}</div>
			<div className={styles.monthDate}>{monthDate(endDate)}</div>
		</div>
	)
}

Date.propTypes = {
	startDate: PropTypes.instanceOf(Date),
	endDate: PropTypes.instanceOf(Date),
	backgroundColor: PropTypes.string
}
