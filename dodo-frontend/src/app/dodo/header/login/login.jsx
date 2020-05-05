import React from "react"
import styles from "./login.module.scss"
import { useTranslation } from "react-i18next"
import { Link } from "react-router-dom"

export const Login = () => {
	const { t } = useTranslation("ui")

	return (
		<Link to="/login" className={styles.login}>
			{t("header_sign_in_text")}
		</Link>
	)
}
