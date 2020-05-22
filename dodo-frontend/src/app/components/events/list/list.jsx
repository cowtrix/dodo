import React from "react"
import PropTypes from "prop-types"
import { Summary } from "../summary"

import styles from "./list.module.scss"

export const List = ({ events = [] }) =>
	events.length ? (
		<ul className={styles.eventList}>
			{events.map(event => (
				<Summary {...event} key={event.guid} />
			))}
		</ul>
	) : null

List.propTypes = {
	events: PropTypes.array
}
