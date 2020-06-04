import React, { Fragment } from "react"
import PropTypes from "prop-types"
import { Summary } from "../summary"

import styles from "./list.module.scss"

export const List = ({ events = [], title, resourceTypes }) =>
	events.length ?
		<Fragment>
			<h3 className={styles.title}>{title}</h3>
			<div className={styles.listBox}>
				<ul className={styles.eventList}>
					{events.map(event => (
						<Summary {...event} key={event.guid} resourceTypes={resourceTypes}/>
					))}
				</ul>
			</div>
		</Fragment>
		: null

List.propTypes = {
	events: PropTypes.array,
	resourceTypes: PropTypes.array
}
