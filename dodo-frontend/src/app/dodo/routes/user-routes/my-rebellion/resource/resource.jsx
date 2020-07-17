import React from 'react'
import PropTypes from 'prop-types'
import { Link } from 'react-router-dom'
import styles from './resource.module.scss'
import { Icon } from 'app/components/index'


export const Resource = ({ name, guid, type, resourceTypes }) => {
	const resourceType = resourceTypes.find(thisType => thisType.value === type)
	const backgroundColor = resourceType && '#' + resourceType.displayColor

	return (
		<Link
			to={type + '/' + guid}
			className={`${styles.resource} ${styles[type]}`}
			style={{
				backgroundColor: backgroundColor}}>
			<h3>{name}</h3>
			<Icon icon="chevron-right" size="1x" className={styles.icon} />
		</Link>
	)
}


Resource.propTypes = {
	name: PropTypes.string,
	guid: PropTypes.string,
	type: PropTypes.string,
	resourceTypes: PropTypes.object,
}