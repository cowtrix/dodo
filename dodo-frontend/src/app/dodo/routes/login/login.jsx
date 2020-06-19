import React, { Fragment, useState } from "react"
import { useTranslation } from "react-i18next"
import { postLogin } from "app/domain/services/login"
import { useHistory } from 'react-router-dom'


import { Input, Container, Submit, Error } from 'app/components/forms'

const LOGIN = 'Login'

export const Login = ({ login, isLoggedIn, error }) => {

	const history = useHistory()

	if (isLoggedIn) {
		history.push('/')
	}

	const { t } = useTranslation("ui")

	const [username, setUsername] = useState("")
	const [password, setPassword] = useState("")

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
						<Error error={error}/>
						<Submit
							value={t("header_sign_in_text")}
							submit={login(username, password)}
						/>
					</Fragment>
				}
			/>
	)
}
