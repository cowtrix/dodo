import React from "react"
import PropTypes from "prop-types"

import { Link } from "react-router-dom"
import { Date as DateTile, Type } from "../../tiles"
import { Title } from "./title"
import { Description } from "./description"
import styles from "./summary.module.scss"

export const Summary = ({
	name,
	startDate,
	endDate,
	publicDescription,
	guid,
	metadata
}) => (
	<li className={styles.eventSummmary}>
		<Link to={`${metadata.type + "/" + guid}`} className={styles.link}>
			{startDate ? (
				<DateTile
					startDate={new Date(startDate)}
					endDate={new Date(endDate)}
				/>
			) : (
				<Type type={metadata.type} />
			)}
			<Title title={name} location={metadata.type} />
			<Description description={publicDescription} />
		</Link>
	</li>
)

Summary.propTypes = {
	Name: PropTypes.string,
	location: PropTypes.string,
	StartDate: PropTypes.string,
	EndDate: PropTypes.string,
	summary: PropTypes.string,
	metada: PropTypes.object.isRequired
}
