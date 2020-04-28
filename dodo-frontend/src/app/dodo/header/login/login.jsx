import React from "react"
import styles from "./login.module.scss"
import { useTranslation } from "react-i18next"

export const Login = () => {
	const { t } = useTranslation("ui")

	return <div className={styles.login}>{t("header_sign_in_text")}</div>
}
