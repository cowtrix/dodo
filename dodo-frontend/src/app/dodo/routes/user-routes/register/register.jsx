import React, { useState } from 'react'
import PropTypes from 'prop-types'
import { Container, Submit, Input, Error } from 'app/components/forms/index'

import styles from './register.module.scss'
import { useHistory } from 'react-router-dom'
import { Loader } from '../../../../components/loader'

const REGISTER = "Register a new account"

const passwordContainsSymbol = (password) => !/^(?=.*[@#$%^&+=!]).*$/.test(password)
const emailRegex = (email) => /\w+@\w+\.\w{2,}/.test(email)
const notEmptyAndLengthBelow = (minLength, str) => !!str && str.length < minLength

export const Register = ({ register, isLoggedIn, registeringUser, error }) => {
	const history = useHistory()

	if (isLoggedIn) {
		history.push('/')
	}

	const [username, setUsername] = useState("")
	const [name, setName] = useState("")
	const [email, setEmail] = useState("")
	const [password, setPassword] = useState("")
	const [passwordConfirmation, setPasswordConfirmation] = useState("")

	const usernameShort = notEmptyAndLengthBelow(3, username)
	const nameShort = notEmptyAndLengthBelow(3, name)
	const emailInvalid = !!email && !emailRegex(email)
	const passwordShort = notEmptyAndLengthBelow(8, password)
	const passwordInvalid = password.length > 0 && passwordContainsSymbol(password)
	const passwordsNotEqual = !!passwordConfirmation && password !== passwordConfirmation
	const hasError = !username.length || !name.length || !email.length || !password.length || !passwordConfirmation.length ||
		usernameShort || nameShort || emailInvalid || passwordShort || passwordInvalid || passwordsNotEqual

	return (
		<Container
			title={REGISTER}
			content={
				<>
					<Loader
						display={registeringUser}
					/>
					{usernameShort ? <Error error="Username should be longer"/> : null}
					<Input
						name="Username"
						id="username"
						type="text"
						value={username}
						setValue={setUsername}
						error={usernameShort}
						maxLength={63}
					/>
					{nameShort ? <Error error="Name should be longer"/> : null}
					<Input
						name="Name"
						id="name"
						type="text"
						value={name}
						setValue={setName}
						error={nameShort}
						maxLength={63}
					/>
					{emailInvalid ? <Error error="Email is invalid"/> : null}
					<Input
						name="Email"
						id="email"
						type="email"
						value={email}
						setValue={setEmail}
						error={emailInvalid}
						maxLength={253}
					/>
					{passwordShort ? <Error error="Password should be longer"/> : null}
					{passwordInvalid ? <Error error="Password should contain a symbol"/> : null}
					<Input
						name="Password"
						id="password"
						type="password"
						value={password}
						setValue={setPassword}
						error={passwordShort || passwordInvalid}
						maxLength={63}
					/>
					{passwordsNotEqual ? <Error error="Passwords should match"/> : null}
					<Input
						name="Confirm Password"
						id="confirmPassword"
						type="password"
						value={passwordConfirmation}
						setValue={setPasswordConfirmation}
						error={passwordsNotEqual}
						maxLength={63}
					/>
					<p>
						By continuing, you agree to the Rebel Agreement and Privacy Policy.
					</p>
					<Error
					 error={error ? error.status + ' ' + error.title : null}
					/>
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
				</>
			}
		/>
	)
}

Register.propTypes = {
	register: PropTypes.func,
	isLoggedIn: PropTypes.string,
}
