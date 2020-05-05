import React from "react"
import styles from "./login.module.scss"
import { useTranslation } from "react-i18next"

export const Login = () => {
	const { t } = useTranslation("ui")

	return (
		<div className={styles.loginForm}>
			<form>
				<label for="username">Username</label>
				<input type="text" id="username" name="username"></input>
				<label for="password">Password</label>
				<input type="password" id="password" name="password"></input>
				<input type="submit" value={t("header_sign_in_text")} />
			</form>
		</div>
	)
}
