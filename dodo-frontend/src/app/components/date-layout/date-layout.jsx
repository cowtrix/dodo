import React from "react"
import PropTypes from "prop-types"

import { DateTile } from "app/components/date-tile"
import styles from "./date-layout.module.scss"

export const DateLayout = ({ startDate, endDate, title, children }) => {
	return (
		<div className={styles.DateLayout}>
			<div className={styles.heading}>
				{startDate && (
					<div className={styles.dateTile}>
						<DateTile startDate={startDate} endDate={endDate} />
					</div>
				)}
				<div className={styles.title}>{title}</div>
			</div>
			<div className={styles.content}>{children}</div>
		</div>
	)
}

DateLayout.propTypes = {
	title: PropTypes.node.isRequired,
	children: PropTypes.node,
	startDate: PropTypes.instanceOf(Date),
	endDate: PropTypes.instanceOf(Date)
}
