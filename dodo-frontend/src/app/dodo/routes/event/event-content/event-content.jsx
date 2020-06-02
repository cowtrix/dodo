import React from 'react'
import PropTypes from 'prop-types'
import { Loader, DateLayout, PageTitle } from "app/components"
import { List, Header } from "app/components/events"

import styles from './event-content.module.scss'

export const EventContent = ({ event, setCenterMap}) =>
	<div className={styles.event}>
		<Header event={event} setCenterMap={setCenterMap} />
			<div className={styles.detail}>
				{event.publicDescription}
			</div>
		<List events={event.sites} />
		<List events={event.workingGroups} />
	</div>

EventContent.propTypes = {
	event: PropTypes.object,
	setCenterMap: PropTypes.func
}