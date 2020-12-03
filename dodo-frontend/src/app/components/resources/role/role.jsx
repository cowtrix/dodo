import React, { useState } from 'react'
import PropTypes from 'prop-types'
import styles from './role.module.scss'

const DEFAULT_GUID = '00000000-0000-0000-0000-000000000000'

export const Role = ({ applicantQuestion, isLoggedIn, applyForRole, resourceColor, hasApplied }) => {
	return (
		isLoggedIn && applicantQuestion && hasApplied == DEFAULT_GUID ?
			<div className={styles.applicationFrameContainer}>
				<iframe id="applicationframe" className={styles.applicationFrame} src={'/roleapplication/' + (hasApplied != DEFAULT_GUID ? hasApplied : 'create') + '?header=false'} />
			</div>
			: null
	)
}


Role.propTypes = {
	applicantQuestion: PropTypes.string,
	hasApplied: PropTypes.bool,
	isLoggedIn: PropTypes.bool,
	applyForRole: PropTypes.func,
	resourceColor: PropTypes.string,
}
