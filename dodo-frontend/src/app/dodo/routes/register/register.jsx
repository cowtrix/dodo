import React, { Fragment, useState } from 'react'
import PropTypes from 'prop-types'
import { Container, Submit, Input, Error } from 'app/components/forms/'

import styles from './register.module.scss'
import { useHistory } from 'react-router-dom'


const REGISTER = "Register for XR"

export const Register = ({ register, isLoggedIn }) => {
	const history = useHistory()

	if (isLoggedIn) {
		history.push('/')
	}

	const [username, setUsername] = useState("")
	const [name, setName] = useState("")
	const [email, setEmail] = useState("")
	const [password, setPassword] = useState("")
	const [passwordConfirmation, setPasswordConfirmation] = useState("")

	const userNameLength = username.length < 3 && username.length
	const nameLength = name.length < 3 && name.length
	const passwordLength = password.length < 8 && password.length
	const passwordValid = /[-!$%^&*()_+|~=`{}\[\]:";'<>?,.\/]/.test(password) || !password.length
	const passwordMatch = passwordConfirmation.length && password !== passwordConfirmation
	const emailValidation = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email) || !email.length
	const hasError =
		userNameLength || nameLength || passwordLength || passwordMatch || !passwordValid || !emailValidation ||
		!username.length || !name.length || !email.length || !password.length || !passwordConfirmation.length

	return (
		<Container
			title={REGISTER}
			content={
				<Fragment>
					{userNameLength ? <Error error="Username should be longer"/> : null}
					<Input
						name="Username"
						id="username"
						type="text"
						value={username}
						setValue={setUsername}
						error={userNameLength}
					/>
					{nameLength ? <Error error="Name should be longer"/> : null}
					<Input
						name="Name"
						id="name"
						type="text"
						value={name}
						setValue={setName}
						error={nameLength}
					/>
					{!emailValidation ? <Error error="Email is invalid"/> : null}
					<Input
						name="Email"
						id="email"
						type="email"
						value={email}
						setValue={setEmail}
						error={!emailValidation}
					/>
					{passwordLength ? <Error error="Password should be longer"/> : null}
					{!passwordValid ? <Error error="Password should contain symbol"/> : null}
					<Input
						name="Password"
						id="password"
						type="password"
						value={password}
						setValue={setPassword}
						error={passwordLength || !passwordValid}
					/>
					{passwordMatch ? <Error error="Passwords should match"/> : null}
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
					<Submit
						className={hasError ? styles.disabled : null}
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
	isLoggedIn: PropTypes.string,
}