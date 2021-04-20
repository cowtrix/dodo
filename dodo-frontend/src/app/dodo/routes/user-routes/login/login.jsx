import { Container, Error, Input, Submit, TickBox } from "app/components/forms";
import PropTypes from "prop-types";
import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { Link, useHistory, useLocation } from "react-router-dom";

import {
	getReturnPath,
	isRouterRoute,
	keepReturnPathParam,
} from "../../../../domain/services/services";

export const Login = ({ login, isLoggedIn, error, isLoggingIn }) => {
	const history = useHistory();
	const location = useLocation();

	if (isLoggedIn) {
		const returnPath = getReturnPath(location) || "/";

		if (isRouterRoute(returnPath)) {
			history.replace(returnPath);
		} else {
			window.location = returnPath;
		}
	}

	const { t } = useTranslation("ui");

	const [username, setUsername] = useState("");
	const [password, setPassword] = useState("");
	const [rememberMe, setRememberMe] = useState(true);

	const handleLogin = (event) => {
		event.preventDefault();
		login(username, password, rememberMe);
	}

	return (
		<Container
			content={
				<form noValidate onSubmit={handleLogin}>
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
						Not a member?{' '}
						<Link to={keepReturnPathParam("/register", location)}>
							Click here to register
						</Link>
					</p>
					<p>
						Forgot your password?{' '}
						<Link to="/reset-password">Click here</Link>
					</p>
					<Error error={error} />
					<Submit value={t("sign_in_button_text")} />
				</form>
			}
			loading={isLoggingIn}
		/>
	);
};

Login.propTypes = {
	login: PropTypes.func,
	isLoggedIn: PropTypes.bool,
	isLoggingIn: PropTypes.bool,
	error: PropTypes.object,
};
