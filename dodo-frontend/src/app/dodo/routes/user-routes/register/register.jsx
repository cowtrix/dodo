import styles from "./register.module.scss";
import { useURLParams } from "app/hooks/useURLParams";
import React, { useState, useEffect } from 'react'
import PropTypes from 'prop-types'
import { Container, Submit, Input, Error, TickBox } from 'app/components/forms/index'
import { emailIsValid, getReturnPath, passwordContainsNoSymbol, strNotEmptyAndLengthBelow } from 'app/domain/services/services';
import { useTranslation } from 'react-i18next';
import { useHistory, useLocation } from 'react-router-dom'
import { Loader } from '../../../../components/loader'
import { APP_TITLE } from '../../../../constants'

export const Register = ({ register, isLoggedIn, registeringUser, error, privacyPolicy, rebelAgreement }) => {
	const history = useHistory()
	const location = useLocation()
	const { token } = useURLParams();

	const { t } = useTranslation("ui")
	if (isLoggedIn) {
		history.push(getReturnPath(location) || "/");
	}

	const [username, setUsername] = useState("");
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [passwordConfirmation, setPasswordConfirmation] = useState("");
	const [dailyUpdate, setDailyUpdate] = useState(false);
	const [weeklyUpdate, setWeeklyUpdate] = useState(false);
	const [newNotifications, setNewNotifications] = useState(false);
	const [acceptTerms, setAcceptTerms] = useState(false);

	const usernameShort = strNotEmptyAndLengthBelow(3, username)
	const emailInvalid = !!email && !emailIsValid(email)
	const passwordShort = strNotEmptyAndLengthBelow(8, password)
	const passwordInvalid = password.length > 0 && passwordContainsNoSymbol(password)
	const passwordsNotEqual = !!passwordConfirmation && password !== passwordConfirmation
	const hasError = !username.length || !email.length || !password.length || !passwordConfirmation.length ||
		usernameShort || emailInvalid || passwordShort || passwordInvalid || passwordsNotEqual || !acceptTerms

	const validationErrors = error?.response?.errors || {};

	const getValidationMessage = (fieldName) => {
		return Array.isArray(validationErrors[fieldName])
			? validationErrors[fieldName][0]
			: validationErrors[fieldName];
	};

	useEffect(() => {
		document.title = APP_TITLE + " | Register"
	}, []);

	return (
		<Container
			content={
				<>
					<Loader display={registeringUser} />
					{token && (
						<div className={styles.tokenBox}>
							<h3>Invite token:</h3>
							<p>{token}</p>
						</div>
					)}
					{usernameShort ? (
						<Error error="Username should be longer" />
					) : null}
					<Input
						name={t("Username")}
						id="username"
						type="text"
						value={username.toLocaleLowerCase().replace(/[^a-z0-9_]*/g, "")}
						setValue={setUsername}
						error={usernameShort || !!validationErrors["Username"]}
						maxLength={63}
						message={getValidationMessage("Username")}
						placeholder="Must be all lowercase, alphanumeric and _"
					/>
					{emailInvalid ? <Error error="Email is invalid" /> : null}
					<Input
						name={t("Email")}
						id="email"
						type="email"
						value={email}
						setValue={setEmail}
						error={emailInvalid || !!validationErrors["Email"]}
						maxLength={253}
						message={getValidationMessage("Email")}
						placeholder="An email will be sent here to verify this address"
					/>
					{passwordShort ? (
						<Error error="Password should be longer" />
					) : null}
					{passwordInvalid ? (
						<Error error="Password should contain a symbol" />
					) : null}
					<Input
						name={t("Password")}
						id="password"
						type="password"
						value={password}
						setValue={setPassword}
						error={
							passwordShort ||
							passwordInvalid ||
							!!validationErrors["Password"]
						}
						maxLength={63}
						message={getValidationMessage("Password")}
					/>
					{passwordsNotEqual ? (
						<Error error="Passwords should match" />
					) : null}
					<Input
						name={t("Confirm Password")}
						id="confirmPassword"
						type="password"
						value={passwordConfirmation}
						setValue={setPasswordConfirmation}
						error={
							passwordsNotEqual || !!validationErrors["Password"]
						}
						maxLength={63}
					/>
					{/* <TickBox
						id="dailyUpdate"
						checked={dailyUpdate}
						setValue={val => setDailyUpdate(val)}
						useAriaLabel={true}
						message={
							<>
								Please send me daily updates via email
							</>
						}
					/>
					<TickBox
						id="weeklyUpdate"
						checked={weeklyUpdate}
						setValue={val => setWeeklyUpdate(val)}
						useAriaLabel={true}
						message={
							<>
								Please send me weekly updates via email
							</>
						}
					/> */}
					<TickBox
						id="newNotifications"
						checked={newNotifications}
						setValue={val => setNewNotifications(val)}
						useAriaLabel={true}
						message={
							<>
								Please send me new notifications via email
							</>
						}
					/>
					<div className={`${styles.termsBox}`}>
						<TickBox
							id="termsConditions"
							checked={acceptTerms}
							setValue={val => setAcceptTerms(val)}
							useAriaLabel={true}
							message={
								<>
									{t("I agree to the")}{" "}
									<a
										href={rebelAgreement}
										target="_blank"
										rel="noreferrer noopener"
									>
										{t("Rebel Agreement")}
									</a>{" "}
									{t("and")}{" "}
									<a
										href={privacyPolicy}
										target="_blank"
										rel="noreferrer noopener"
									>
										{t("Privacy Policy")}
									</a>
									.
								</>
							}
						/>
					</div>
					<Error error={error} />
					<Submit
						disabled={hasError}
						submit={register({
							username,
							password,
							email,
							token,
							dailyUpdate,
							weeklyUpdate,
							newNotifications
						})}
						value={t("Register a new account")}
					/>
				</>
			}
		/>
	);
};

Register.propTypes = {
	register: PropTypes.func,
	isLoggedIn: PropTypes.string,
};
