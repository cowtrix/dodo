import React, { useState } from 'react'
import PropTypes from 'prop-types'
import { Button } from 'app/components/button';
import styles from './role.module.scss'

const DEFAULT_GUID = '00000000-0000-0000-0000-000000000000'

export const Role = ({ isLoggedIn, resource, hasApplied, isEmailConfirmed, resendVerificationEmail }) => {
	return (
		isLoggedIn ? (
			<div className={styles.applicationFrameContainer}>
				{isEmailConfirmed ? (
					<iframe id="applicationframe" className={styles.applicationFrame} src={'/roleapplication/' + (hasApplied != DEFAULT_GUID ? hasApplied : resource.guid + '/apply') + '?header=false'} />
				) : (
					<p className={styles.confirmEmail}>
						You must confirm your email address before you can apply for a role.
						<Button onClick={resendVerificationEmail} className={styles.resendEmailButton}>
							Resend Verification Email
						</Button>
					</p>
				)}
			</div>
		) : null
	)
}

Role.propTypes = {
	hasApplied: PropTypes.bool,
	isLoggedIn: PropTypes.bool,
	resourceColor: PropTypes.string,
}
