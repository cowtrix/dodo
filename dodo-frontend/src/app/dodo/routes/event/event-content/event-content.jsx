import React from 'react'
import PropTypes from 'prop-types'
import { Loader, DateLayout, PageTitle } from "app/components"
import { List } from "app/components/events"


import styles from './event-content.module.scss'

export const EventContent = ({ event}) =>
	<div className={styles.event}>
		<DateLayout
			startDate={
				event.startDate
					? new Date(event.startDate)
					: null
			}
			endDate={
				event.endDate
					? new Date(event.endDate)
					: null
			}
			title={
				<PageTitle
					title={event.name}
					subTitle={event.metadata.type}
				/>
			}
		>
			<div className={styles.detail}>
				{event.publicDescription}
			</div>
		</DateLayout>
		<List events={event.sites} />
		<List events={event.workingGroups} />
	</div>

EventContent.propTypes = {
	event: PropTypes.object
}