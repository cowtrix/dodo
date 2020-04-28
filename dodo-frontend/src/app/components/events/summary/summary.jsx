import React from "react"
import PropTypes from "prop-types"

import { Link } from "react-router-dom"
import { DateTile } from "../../date-tile"
import { Title } from "./title"
import { Description } from "./description"
import styles from "./summary.module.scss"

export const Summary = ({
	name,
	startDate,
	endDate,
	publicDescription,
	guid
}) => (
	<li className={styles.eventSummmary}>
		<Link to={"/rebellion/" + guid} className={styles.link}>
			<DateTile
				startDate={new Date(startDate)}
				endDate={new Date(endDate)}
			/>
			<Title title={name} location="Glasgow" />
			<Description description={publicDescription} />
		</Link>
	</li>
)

Summary.propTypes = {
	Name: PropTypes.string,
	location: PropTypes.string,
	StartDate: PropTypes.string,
	EndDate: PropTypes.string,
	summary: PropTypes.string
}
