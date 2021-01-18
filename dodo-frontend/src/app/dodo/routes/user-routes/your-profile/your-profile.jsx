import React, { useState, useEffect } from 'react'
import PropTypes from 'prop-types'
import { Container, EditableInput, Submit, Error } from 'app/components/forms/index'
import { UpdateEmail } from './update-email/index'
import useRequireLogin from 'app/hooks/useRequireLogin';

const MY_REBELLION = "Your profile"
const UPDATE_DETAILS = "Update my details"

export const YourProfile = (
	{
		currentUsername, currentEmail, fetchingUser, updateDetails, isConfirmed, isUpdating, updateError, guid, resendVerificationEmail }
	) => {

	const [username, setUserName] = useState(currentUsername)
	const [email, setEmail] = useState(currentEmail)
	const [firstRun, setFirstRun] = useState(undefined);

	useEffect(() => {
		setUserName(currentUsername)
		setEmail(currentEmail)
	}, [currentUsername, currentEmail])

	const handleUpdate = () => {
		updateDetails(username, email, guid);
		setFirstRun(false);
	}

	// Ensure user is logged in
	const loggedIn = useRequireLogin();
	if(!loggedIn) return null;

	return (
		<Container
			loading={fetchingUser || isUpdating}
			title={MY_REBELLION}
			content={
				<>
					<EditableInput
						name="Username"
						id="username"
						type="text"
						value={username}
						setValue={setUserName}
					/>
					<UpdateEmail
						email={email}
						setEmail={setEmail}
						isConfirmed={isConfirmed}
						resendVerificationEmail={resendVerificationEmail}
					/>
					{!firstRun && updateError && (<Error error={updateError} marginTop />)}
					<Submit
						value={UPDATE_DETAILS}
						submit={handleUpdate}
					/>
				</>
			}
		/>
	)
}


YourProfile.propTypes = {
	currentUsername: PropTypes.string
}
