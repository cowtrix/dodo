import React from 'react'
import PropTypes from 'prop-types'

import styles from './error.module.scss'

export const Error = ({ error }) =>
	error ? <div className={styles.error}>
		{error}
	</div> :
		null

Error.propTypes = {
	error: PropTypes.string,
}