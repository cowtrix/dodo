import React from 'react'
import PropTypes from 'prop-types'

import styles from './summary.module.scss'


export const Summary = ({ description }) => {
	const summary = description.slice(0, 200)

	return (
		<div className={styles.summary}>
			{summary}
		</div>
	)
}


Summary.propTypes = {
	description: PropTypes.string,
}