import React from 'react'
import PropTypes from 'prop-types'
import { Link } from 'react-router-dom'
import { Icon } from 'app/components/index'
import styles from './resource.module.scss'

export const Resource = ({ name, slug, metadata, resourceTypes = [], administrator }) => {
	const resourceType = resourceTypes.find(thisType => thisType.value === metadata.type)
	const backgroundColor = resourceType && '#' + resourceType.displayColor

	return (
		<>
			<Link
				to={metadata.type + '/' + slug}
				className={`${styles.resource} ${styles[metadata.type]}`}
				style={{ backgroundColor: backgroundColor}}
				title={`View ${name}`}>
				<h3>{name}</h3>
				<div className={styles.icons}>
					<Icon icon='chevron-right' size='1x' className={styles.icon} />
				</div>
			</Link>
			{administrator && (
				<a
					href={'/edit/' + metadata.type + '/' + slug}
					target='_blank'
					rel='noopener noreferrer'
					className={`${styles.resource} ${styles[metadata.type]} ${styles.edit}`}
					style={{ backgroundColor: backgroundColor }}
					title={`Edit ${name}`}>
					<Icon icon='edit' size='1x' />
				</a>
			)}
		</>
	)
}

Resource.propTypes = {
	name: PropTypes.string,
	slug: PropTypes.string,
	metadata: PropTypes.object,
	resourceTypes: PropTypes.array,
}
