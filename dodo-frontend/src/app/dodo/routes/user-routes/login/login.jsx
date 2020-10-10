import React, { useState } from "react"
import PropTypes from 'prop-types'

import { useTranslation } from "react-i18next"
import { useHistory, Link, useLocation } from 'react-router-dom'

import { Input, Container, Submit, Error, TickBox } from 'app/components/forms/index'

const LOGIN = 'Login'

export const Login = ({ login, isLoggedIn, error, isLoggingIn }) => {

	const history = useHistory()
	const location = useLocation();

	if (isLoggedIn) {
		const url = new URLSearchParams(location.search.replace(/returnurl/ig, 'returnurl'));
		history.push(url.get('returnurl') || '/');
	}

	const { t } = useTranslation("ui")

	const [username, setUsername] = useState("")
	const [password, setPassword] = useState("")
	const [rememberMe, setRememberMe] = useState(true)

	let errorMessage;
	if(error) {
		switch(error.status) {
			case 0:
				errorMessage = t('login_network_error');
				break;
			case 400:
				errorMessage = t('login_user_not_found');
				break;
			default:
				errorMessage = undefined;
		}
	}

	return (
			<Container
				title={LOGIN}
				content={
					<>
						<Input
							name="Username"
							id="username"
							type="text"
							value={username}
							setValue={setUsername}
							maxLength={63}
						/>
						<Input
							name="Password"
							id="password"
							type="password"
							value={password}
							setValue={setPassword}
							maxLength={63}
						/>
						<TickBox
							name="Remember Me"
							id="rememberMe"
							checked={rememberMe}
							setValue={setRememberMe}
						/>
						<p>
							Not a member? <Link to="/register">Click here to register</Link>
						</p>
						<p>
							Forgot your password? <Link to="/reset-password">Click here</Link>
						</p>
						{errorMessage && <Error error={errorMessage}/>}
						<Submit
							value={t("header_sign_in_text")}
							submit={login(username, password, rememberMe)}
						/>
					</>
				}
				loading={isLoggingIn}
			/>
	)
}

Login.propTypes = {
	login: PropTypes.func,
	isLoggedIn: PropTypes.bool,
	isLoggingIn: PropTypes.bool,
	error: PropTypes.object,
}
