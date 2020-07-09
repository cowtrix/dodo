import React, { Fragment, useState, useEffect } from 'react'
import PropTypes from 'prop-types'
import { Container, EditableInput, Submit } from 'app/components/forms'
import { UpdateEmail } from './update-email'

import { useHistory } from 'react-router-dom'

const MY_REBELLION = "Your profile"
const UPDATE_DETAILS = "Update my details"

export const YourProfile = ({ currentUsername, currentName, currentEmail, fetchingUser, updateDetails, isConfirmed, guid }) => {
	// const history = useHistory()

	// if (!currentUsername) {
	// 	history.push('/register')
	// }

	const [username, setUserName] = useState(currentUsername)
	const [name, setName] = useState(currentName)
	const [email, setEmail] = useState(currentEmail)

	useEffect(() => {
		setUserName(currentUsername)
		setName(currentName)
		setEmail(currentEmail)
	}, [currentUsername, currentEmail, currentName])

	return (
		<Container
			loading={fetchingUser}
			title={MY_REBELLION}
			content={
				<Fragment>
					<EditableInput
						name="Username"
						id="username"
						type="text"
						value={username}
						setValue={setUserName}
					/>
					<EditableInput
						name="Name"
						id="name"
						type="text"
						value={name}
						setValue={setName}
					/>
					<UpdateEmail email={email} setEmail={setEmail} isConfirmed={isConfirmed} />
					<Submit
						value={UPDATE_DETAILS}
						submit={updateDetails(username, name, email, guid)}
					/>
				</Fragment>
			}
		/>
	)
}


YourProfile.propTypes = {
	currentUsername: PropTypes.string
}