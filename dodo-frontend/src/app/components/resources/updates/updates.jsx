import React from 'react'
import PropTypes from 'prop-types'

import styles from './updates.module.scss'

export const Updates = ({ notifications }) =>
	<div className={styles.container}>
		{notifications.map(({message}) => (
			<div>
				{message}
			</div>
		))}
	</div>

Updates.propTypes = {
	description: PropTypes.string,
}
