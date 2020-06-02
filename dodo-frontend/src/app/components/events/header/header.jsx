import React from 'react'
import PropTypes from 'prop-types'
import { CenterMap } from './center-map'
import { Countdown } from './countdown'
import { Title } from './title'
import { Dates } from './dates'

import styles from './header.module.scss'

export const Header = ({ event, setCenterMap }) =>
	<div className={styles.header}>
		<div className={styles.headerLeft}>
			<Title name={event.name}/>
			<Dates startDate={event.startDate} endDate={event.endDate} />
		</div>
		<div className={styles.headerRight}>
			<CenterMap setCenterMap={setCenterMap} />
			<Countdown startDate={event.startDate} />
		</div>
	</div>

Header.propTypes = {
	event: PropTypes.object
}