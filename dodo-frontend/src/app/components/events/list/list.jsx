import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { Summary } from '../summary'

import styles from './list.module.scss'

export const List = ({ events }) =>
	<ul className={styles.eventList}>
		{events.map(event =>
			<Summary {...event} />
		)}
	</ul>

List.propTypes = {
	events: PropTypes.array,
}