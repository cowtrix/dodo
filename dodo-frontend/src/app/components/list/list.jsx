import React, { Fragment } from "react"
import PropTypes from "prop-types"
import { Summary } from "../resources/summary/index"

import styles from "./list.module.scss"

export const List = ({ resources = [], title, resourceTypes }) =>
	resources.length ?
		<Fragment>
			{title ? <h3 className={styles.title}>{title}</h3> : null}
			<div className={styles.listBox}>
				<ul className={styles.eventList}>
					{resources.map(event => (
						<Summary {...event} key={event.guid} resourceTypes={resourceTypes}/>
					))}
				</ul>
			</div>
		</Fragment>
		: null

List.propTypes = {
	title: PropTypes.string,
	resources: PropTypes.array,
	resourceTypes: PropTypes.array
}
