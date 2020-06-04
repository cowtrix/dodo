import React from 'react'
import PropTypes from 'prop-types'
import { Loader, DateLayout, PageTitle } from "app/components"
import { List, Header, Description, SignUpButton } from "app/components/events"

import styles from './event-content.module.scss'

const JOIN_US_SITES = "Join us at a protest site"
const COME_TO_EVENT = "Come to an event"
const VOLUNTEER_NOW = "Volunteer now with a working group"

export const EventContent = ({ event, setCenterMap, resourceTypes}) =>
	<div className={styles.event}>
		<Header event={event} setCenterMap={setCenterMap} />
		<Description description={event.publicDescription} />
		<SignUpButton />
		<List events={event.events} title={COME_TO_EVENT} resourceTypes={resourceTypes} />
		<List events={event.sites} title={JOIN_US_SITES} resourceTypes={resourceTypes} />
		<List events={event.workingGroups} title={VOLUNTEER_NOW} resourceTypes={resourceTypes} />
	</div>

EventContent.propTypes = {
	event: PropTypes.object,
	resourceTypes: PropTypes.array,
	setCenterMap: PropTypes.func
}