import React from 'react'
import PropTypes from 'prop-types'
import { EditableInput } from 'app/components/forms'
import styles from './update-email.module.scss'

export const EMAIL_WARNING = "Your email has not been verified, which will limit what you can do. Resend Verification Email"

export const UpdateEmail = ({ email, setEmail, isConfirmed = true }) =>
	<div className={!isConfirmed ? styles.confirmed : ''}>
		<EditableInput
			name="Email"
			id="email"
			type="text"
			value={email}
			setValue={setEmail}
		/>
		{!isConfirmed ?
			<div className={styles.warning}>
				{EMAIL_WARNING}
			</div>
			:
		null}
	</div>

UpdateEmail.propTypes = {
	email: PropTypes.string,
	setEmail: PropTypes.func,
}