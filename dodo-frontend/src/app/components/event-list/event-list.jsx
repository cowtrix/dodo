import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { EventSummary } from '../event-summary'

import styles from './event-list.module.scss'

export const EventList = ({ events }) =>
	<ul className={styles.eventList}>
		{events.map(event =>
			<EventSummary {...event} />
		)}
	</ul>

EventList.propTypes = {
	events: PropTypes.array,
}