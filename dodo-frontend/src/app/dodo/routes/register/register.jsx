import React, { Fragment, useState } from 'react'
import PropTypes from 'prop-types'
import { Container, Submit, Input } from 'app/components/forms/'
import { Error } from '../../../components/forms/error'

const REGISTER = "Register for XR"

export const Register = ({ register }) => {
	const [username, setUsername] = useState("")
	const [name, setName] = useState("")
	const [email, setEmail] = useState("")
	const [password, setPassword] = useState("")
	const [passwordConfirmation, setPasswordConfirmation] = useState("")

	const userNameLength = username.length < 3 && username.length > 0
	const nameLength = name.length < 3 && name.length > 0
	const passwordLength = password.length < 8 && password.length > 0
	const passwordMatch = passwordConfirmation.length && password !== passwordConfirmation

	return (
		<Container
			title={REGISTER}
			content={
				<Fragment>
					<Input
						name="Username"
						id="username"
						type="text"
						value={username}
						setValue={setUsername}
						error={userNameLength}
					/>
					<Input
						name="Name"
						id="name"
						type="text"
						value={name}
						setValue={setName}
						error={nameLength}
					/>
					<Input
						name="Email"
						id="email"
						type="email"
						value={email}
						setValue={setEmail}
					/>
					<Input
						name="Password"
						id="password"
						type="password"
						value={password}
						setValue={setPassword}
						error={passwordLength}
					/>
					<Input
						name="Confirm Password"
						id="confirmPassword"
						type="password"
						value={passwordConfirmation}
						setValue={setPasswordConfirmation}
						error={passwordMatch}
					/>
					<p>
						By continuing, you agree to the Rebel Agreement and Privacy Policy.
					</p>
					{userNameLength && <Error error="Username should be longer"/>}
					{nameLength && <Error error="Name should be longer"/>}
					{passwordLength && <Error error="Password should be longer"/>}
					{passwordMatch ? <Error error="Passwords should match"/> : null}

					<Submit
							submit={register({
							username,
							password,
							email,
							name,
						})}
						value={REGISTER}
					/>
				</Fragment>
			}
		/>
	)
}


Register.propTypes = {
	register: PropTypes.func,
}