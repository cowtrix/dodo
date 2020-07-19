import React, { Fragment, useState } from "react"
import PropTypes from 'prop-types'

import { useTranslation } from "react-i18next"
import { useHistory, Link } from 'react-router-dom'

import { Input, Container, Submit, Error, TickBox } from 'app/components/forms/index'

const LOGIN = 'Login'

export const Login = ({ login, isLoggedIn, error, isLoggingIn }) => {

	const history = useHistory()

	if (isLoggedIn) {
		history.push('/')
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
						/>
						<Input
							name="Password"
							id="password"
							type="password"
							value={password}
							setValue={setPassword}
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