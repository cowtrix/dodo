import React from 'react'
import PropTypes from 'prop-types'

import { DateTile } from '../date-tile'
import { Title } from './title'
import { Summary } from './summary'
import styles from './event-summary.module.scss'
import { Button } from '../button'


export const EventSummary = ({ Name, location = 'Glasgow', StartDate, EndDate, PublicDescription, GUID }) =>
	<li className={styles.eventSummmary}>
		<Button to={'/rebellion/' + GUID} className={styles.link}>
			<DateTile startDate={new Date(StartDate)} endDate={new Date(EndDate)}/>
			<Title title={Name} location={location}/>
			<Summary description={PublicDescription}/>
		</Button>
	</li>

EventSummary.propTypes = {
	Name: PropTypes.string,
	location: PropTypes.string,
	StartDate: PropTypes.string,
	EndDate: PropTypes.string,
	summary: PropTypes.string,
}