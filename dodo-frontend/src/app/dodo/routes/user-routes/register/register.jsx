import React, { useState } from 'react'
import PropTypes from 'prop-types'
import { Container, Submit, Input, Error, TickBox } from 'app/components/forms/index'
import { emailIsValid, getReturnPath, passwordContainsNoSymbol, strNotEmptyAndLengthBelow } from 'app/domain/services/services';
import { useTranslation } from 'react-i18next';
import { useHistory, useLocation } from 'react-router-dom'
import { Loader } from '../../../../components/loader'

export const Register = ({ register, isLoggedIn, registeringUser, error, privacyPolicy, rebelAgreement }) => {
	const history = useHistory()
	const location = useLocation()
	const { t } = useTranslation("ui")
	if (isLoggedIn) {
		history.push(getReturnPath(location) ||'/')
	}

	const [username, setUsername] = useState("")
	const [name, setName] = useState("")
	const [email, setEmail] = useState("")
	const [password, setPassword] = useState("")
	const [passwordConfirmation, setPasswordConfirmation] = useState("")
	const [acceptTerms, setAcceptTerms] = useState(false);

	const usernameShort = strNotEmptyAndLengthBelow(3, username)
	const nameShort = strNotEmptyAndLengthBelow(3, name)
	const emailInvalid = !!email && !emailIsValid(email)
	const passwordShort = strNotEmptyAndLengthBelow(8, password)
	const passwordInvalid = password.length > 0 && passwordContainsNoSymbol(password)
	const passwordsNotEqual = !!passwordConfirmation && password !== passwordConfirmation
	const hasError = !username.length || !name.length || !email.length || !password.length || !passwordConfirmation.length ||
		usernameShort || nameShort || emailInvalid || passwordShort || passwordInvalid || passwordsNotEqual || !acceptTerms

	const validationErrors = error?.response?.errors || {};

	const getValidationMessage = fieldName => {
		return Array.isArray(validationErrors[fieldName]) ? validationErrors[fieldName][0] : validationErrors[fieldName];
	}

	return (
		<Container
			content={
				<>
					<Loader
						display={registeringUser}
					/>
					{usernameShort ? <Error error="Username should be longer"/> : null}
					<Input
						name={t("Username")}
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
						name={t("Name")}
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
						name={t("Email")}
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
						name={t("Password")}
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
						name={t("Confirm Password")}
						id="confirmPassword"
						type="password"
						value={passwordConfirmation}
						setValue={setPasswordConfirmation}
						error={passwordsNotEqual || !!validationErrors['Password']}
						maxLength={63}
					/>
					<TickBox
						id="termsConditions"
						value={acceptTerms}
						setValue={val => setAcceptTerms(val)}
						useAriaLabel={true}
						message={<>
							{t("I agree to the")}{' '}
							<a href={rebelAgreement} target="_blank" rel="noreferrer noopener">
								{t("Rebel Agreement")}
							</a>{' '}
							{' '}{t("and")}{' '}
							<a href={privacyPolicy} target="_blank" rel="noreferrer noopener">
								{t("Privacy Policy")}
							</a>
							.
						</>}
					/>
					<Error error={error}/>
					<Submit
						disabled={hasError}
						submit={register({
							username,
							password,
							email,
							name,
						})}
						value={t("Register a new account")}
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
