import React, { useState } from 'react'
import PropTypes from 'prop-types'
import styles from './role.module.scss'

const DEFAULT_GUID = '00000000-0000-0000-0000-000000000000'

export const Role = ({ isLoggedIn, resource, hasApplied }) => {
	return (
		isLoggedIn ?
			<div className={styles.applicationFrameContainer}>
				<iframe id="applicationframe" className={styles.applicationFrame} src={'/roleapplication/' + (hasApplied != DEFAULT_GUID ? hasApplied : resource.guid + '/apply') + '?header=false'} />
			</div>
			: null
	)
}

Role.propTypes = {
	hasApplied: PropTypes.bool,
	isLoggedIn: PropTypes.bool,
	resourceColor: PropTypes.string,
}
