import React from 'react'
import PropTypes from 'prop-types'
import { Link } from 'react-router-dom'
import styles from './resource.module.scss'
import { Icon } from 'app/components/index'


export const Resource = ({ name, guid, metadata, resourceTypes = [] }) => {
	const resourceType = resourceTypes.find(thisType => thisType.value === metadata.type)
	const backgroundColor = resourceType && '#' + resourceType.displayColor

	return (
		<Link
			to={metadata.type + '/' + guid}
			className={`${styles.resource} ${styles[metadata.type]}`}
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
	metadata: PropTypes.object,
	resourceTypes: PropTypes.object,
}