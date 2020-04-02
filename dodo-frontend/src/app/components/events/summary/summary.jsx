import React from "react"
import { Link } from "react-router-dom"
import PropTypes from "prop-types"

import { DateTile } from "../../date-tile/index"
import { Title } from "./title/index"
import { Description } from "./description"
import styles from "./summary.module.scss"

export const Summary = ({
	Name,
	location = "Glasgow",
	StartDate,
	EndDate,
	PublicDescription,
	GUID
}) => (
	<li className={styles.eventSummmary}>
		<Link to={"/rebellion/" + GUID} className={styles.link}>
			<DateTile
				startDate={new Date(StartDate)}
				endDate={new Date(EndDate)}
			/>
			<Title title={Name} location={location} />
			<Description description={PublicDescription} />
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
