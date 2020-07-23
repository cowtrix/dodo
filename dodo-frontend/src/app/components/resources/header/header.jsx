import React from 'react'
import PropTypes from 'prop-types'
import { CenterMap } from '../../center-map'
import { Countdown } from './countdown'
import { Title } from './title'
import { Dates } from './dates'

import styles from './header.module.scss'

export const Header = ({ resource, setCenterMap, resourceColor, hideMap }) =>
	<div className={styles.header} style={{ backgroundColor: resourceColor }}>
		<div className={styles.headerLeft}>
			<Title name={resource.name}/>
			<Dates startDate={resource.startDate} endDate={resource.endDate} />
		</div>
		<div className={styles.headerRight}>
			<CenterMap setCenterMap={setCenterMap} display={!hideMap} />
			<Countdown startDate={resource.startDate} name={resource.name} />
		</div>
	</div>

Header.propTypes = {
	hideMap: PropTypes.bool,
	resource: PropTypes.object,
	setCenterMap: PropTypes.func,
	resourceColor: PropTypes.string
}