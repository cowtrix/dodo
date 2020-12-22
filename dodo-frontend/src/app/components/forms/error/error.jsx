import React from 'react'
import PropTypes from 'prop-types'

import styles from './error.module.scss'
import { props } from 'ramda';

export const Error = ({ error, marginTop }) => {
	let text;
	if(error?.response?.title) {
		text = error.response?.title;
	}
	else if(typeof error?.response === 'string') {
		text = error.response;
	}
	else {
		text = error?.message || error;
	}

	return error
		? <div className={`${styles.error} ${marginTop ? styles.marginTop : ''}`}>{text}</div>
		: null
};

Error.propTypes = {
	error: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
	marginTop: PropTypes.bool,
}
