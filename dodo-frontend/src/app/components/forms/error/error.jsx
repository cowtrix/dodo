import React from 'react'
import PropTypes from 'prop-types'

import styles from './error.module.scss'

export const Error = ({ error }) => (
	error
		? <div className={styles.error}>
			{(error.response && error.response.title) || error.message || error}
		</div>
		: null
)

Error.propTypes = {
	error: PropTypes.oneOfType([PropTypes.string, PropTypes.object])
}