import React, { Fragment, useState } from 'react'
import { Container, Input, Submit } from 'app/components/forms'
import { api } from 'app/domain/services'
import { RESET_PASSWORD} from 'app/domain/urls'


const RESET_PASSWORD_COPY = "Reset my password"


export const ResetPassword = ({ resetPassword }) => {
	const [ email, setEmail ] = useState("")
	const [ emailSent, setEmailSent ] = useState(false)

	return (
		<Container
			title={RESET_PASSWORD_COPY}
			content={
				<Fragment>
					<p>
						Enter the email associated with your account to request a one-time password reset code.
					</p>
					<p>
						Please be aware that resetting your password will remove all adminstrative rights associated with your
						account.
					</p>
					<Input
						name="email"
						id="email"
						type="text"
						value={email}
						setValue={setEmail}
					/>
					<Submit
						value={RESET_PASSWORD_COPY}
						submit={() => resetPassword(email, setEmailSent)}
					/>
					<p>{emailSent}</p>
				</Fragment>
			}
		/>
	)
}
