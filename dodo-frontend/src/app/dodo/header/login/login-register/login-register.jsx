import React from 'react'
import { Link } from 'react-router-dom'
import { useTranslation } from "react-i18next"
import styles from './login-register.module.scss'

export const LoginRegister = () => {
	const { t } = useTranslation("ui")

	return (
		<Link to="/login" className={styles.loginRegister}>
			{t("header_sign_in_text")}
		</Link>
	)
}

