import React, { useState } from "react"
import styles from "./login.module.scss"
import { useTranslation } from "react-i18next"
import { postLogin } from "app/domain/services/login"
import { useHistory } from "react-router-dom"

export const Login = () => {
	const { t } = useTranslation("ui")

	const [username, setUsername] = useState("")
	const [password, setPassword] = useState("")

	const history = useHistory()

	return (
		<div className={styles.loginFormWrapper}>
			<div className={styles.loginForm}>
				<label htmlFor="username">Username</label>
				<input
					type="text"
					id="username"
					name="username"
					value={username}
					onChange={e => setUsername(e.target.value)}
				></input>
				<label htmlFor="password">Password</label>
				<input
					type="password"
					id="password"
					name="password"
					value={password}
					onChange={e => setPassword(e.target.value)}
				></input>
				<input
					type="submit"
					value={t("header_sign_in_text")}
					onClick={() => {
						postLogin(username, password)
							.then(response => {
								if (!response.ok) {
									throw new Error(
										"Sign in response was not ok"
									)
								}
								console.log("Sign in success:", response)
								history.push("/")
							})
							.catch(error => {
								console.error("Sign in failure:", error)
							})
					}}
				/>
			</div>
		</div>
	)
}
