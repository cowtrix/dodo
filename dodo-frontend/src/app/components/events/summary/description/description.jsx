import React from 'react'
import PropTypes from 'prop-types'

import styles from './description.module.scss'


export const Description = ({ description }) => {
	const summary = description.slice(0, 200)

	return (
		<div className={styles.summary}>
			{summary}
		</div>
	)
}


Description.propTypes = {
	description: PropTypes.string,
}