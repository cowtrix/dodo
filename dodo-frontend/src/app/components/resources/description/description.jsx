import React from 'react'
import PropTypes from 'prop-types'

import styles from './description.module.scss'


export const Description = ({ description }) =>
	<div className={styles.description} dangerouslySetInnerHTML={{__html: description}}/>

Description.propTypes = {
	description: PropTypes.string,
}
