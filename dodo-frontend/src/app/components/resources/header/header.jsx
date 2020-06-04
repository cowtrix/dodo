import React from 'react'
import PropTypes from 'prop-types'
import { CenterMap } from '../../center-map'
import { Countdown } from './countdown'
import { Title } from './title'
import { Dates } from './dates'

import styles from './header.module.scss'

export const Header = ({ resource, setCenterMap }) =>
	<div className={styles.header}>
		<div className={styles.headerLeft}>
			<Title name={resource.name}/>
			<Dates startDate={resource.startDate} endDate={resource.endDate} />
		</div>
		<div className={styles.headerRight}>
			<CenterMap setCenterMap={setCenterMap} />
			<Countdown startDate={resource.startDate} />
		</div>
	</div>

Header.propTypes = {
	event: PropTypes.object
}