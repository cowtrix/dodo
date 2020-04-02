import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import styles from './list-container.module.scss'

export const ListContainer = ({ content }) =>
	<div className={styles.wrapper}>
		<div className={styles.eventListContainer}>
			{content}
		</div>
	</div>

ListContainer.propTypes = {
	content: PropTypes.node,
}