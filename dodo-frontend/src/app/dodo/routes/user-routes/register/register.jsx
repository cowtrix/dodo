import React, { useState } from 'react'
import PropTypes from 'prop-types'
import { Container, Submit, Input, Error } from 'app/components/forms/index'
import { getReturnPath } from '../../../../domain/services/services';

import styles from './register.module.scss'
import { useHistory, useLocation } from 'react-router-dom'
import { Loader } from '../../../../components/loader'

const REGISTER = "Register a new account"

const passwordContainsSymbol = (password) => !/^(?=.*[@#$%^&+=!]).*$/.test(password)
const emailRegex = (email) => /\w+@\w+\.\w{2,}/.test(email)
const notEmptyAndLengthBelow = (minLength, str) => !!str && str.length < minLength

export const Register = ({ register, isLoggedIn, registeringUser, error }) => {
	const history = useHistory()
	const location = useLocation()

	if (isLoggedIn) {
		history.push(getReturnPath(location) ||'/')
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

	const validationErrors = error?.response?.errors || {};

	const getValidationMessage = fieldName => {
		return Array.isArray(validationErrors[fieldName]) ? validationErrors[fieldName][0] : validationErrors[fieldName];
	}

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
						error={usernameShort || !!validationErrors['Username']}
						maxLength={63}
						message={getValidationMessage('Username')}
					/>
					{nameShort ? <Error error="Name should be longer"/> : null}
					<Input
						name="Name"
						id="name"
						type="text"
						value={name}
						setValue={setName}
						error={nameShort || !!validationErrors['Name']}
						maxLength={63}
						message={getValidationMessage('Name')}
					/>
					{emailInvalid ? <Error error="Email is invalid"/> : null}
					<Input
						name="Email"
						id="email"
						type="email"
						value={email}
						setValue={setEmail}
						error={emailInvalid || !!validationErrors['Email']}
						maxLength={253}
						message={getValidationMessage('Email')}
					/>
					{passwordShort ? <Error error="Password should be longer"/> : null}
					{passwordInvalid ? <Error error="Password should contain a symbol"/> : null}
					<Input
						name="Password"
						id="password"
						type="password"
						value={password}
						setValue={setPassword}
						error={passwordShort || passwordInvalid || !!validationErrors['Password']}
						maxLength={63}
						message={getValidationMessage('Password')}
					/>
					{passwordsNotEqual ? <Error error="Passwords should match"/> : null}
					<Input
						name="Confirm Password"
						id="confirmPassword"
						type="password"
						value={passwordConfirmation}
						setValue={setPasswordConfirmation}
						error={passwordsNotEqual || !!validationErrors['Password']}
						maxLength={63}
					/>
					<p>
						By continuing, you agree to the Rebel Agreement and Privacy Policy.
					</p>
					<Error error={error}/>
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
