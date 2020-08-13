import React, { Fragment, useState } from "react"
import PropTypes from 'prop-types'

import { useTranslation } from "react-i18next"
import { useHistory, Link, useLocation } from 'react-router-dom'

import { Input, Container, Submit, Error, TickBox } from 'app/components/forms/index'

const LOGIN = 'Login'

export const Login = ({ login, isLoggedIn, error, isLoggingIn }) => {

	const history = useHistory()
	const location = useLocation();

	if (isLoggedIn) {
		const url = new URLSearchParams(location.search)
		history.push(url.get('ReturnUrl') || url.get('ReturnURL') || '/');
	}

	const { t } = useTranslation("ui")

	const [username, setUsername] = useState("")
	const [password, setPassword] = useState("")
	const [rememberMe, setRememberMe] = useState(true)

	return (
			<Container
				title={LOGIN}
				content={
					<Fragment>
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
							value={rememberMe}
							setValue={setRememberMe}
						/>
						<p>
							Not a member? <Link to="/register">Click here to register</Link>
						</p>
						<p>
							Forgot your password? <Link to="/reset-password">Click here</Link>
						</p>
						<Error error={error}/>
						<Submit
							value={t("header_sign_in_text")}
							submit={login(username, password, rememberMe)}
						/>
					</Fragment>
				}
				loading={isLoggingIn}
			/>
	)
}

Login.propTypes = {
	login: PropTypes.func,
	isLoggedIn: PropTypes.bool,
	isLoggingIn: PropTypes.bool,
	error: PropTypes.string,
}
