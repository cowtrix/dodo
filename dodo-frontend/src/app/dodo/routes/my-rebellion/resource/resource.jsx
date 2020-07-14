import React from 'react'
import PropTypes from 'prop-types'
import { Link } from 'react-router-dom'
import styles from './resource.module.scss'
import { Icon } from 'app/components'


export const Resource = ({ name, guid, type, resourceTypes }) =>
	<Link
		to={type + '/' + guid}
		className={`${styles.resource} ${styles[type]}`}
		style={{
			backgroundColor: '#' + resourceTypes.find(thisType => thisType.value === type).displayColor}}>
		<h3>{name}</h3>
		<Icon icon="chevron-right" size="1x" className={styles.icon} />
	</Link>

Resource.propTypes = {
	name: PropTypes.string,
	guid: PropTypes.string,
	type: PropTypes.string,
	resourceTypes: PropTypes.object,
}