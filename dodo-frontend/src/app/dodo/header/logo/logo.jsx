import React from "react"
import { useTranslation } from "react-i18next"

import { Link } from "react-router-dom"
import logo from "static/XR-logo-full.svg"
import styles from "./logo.module.scss"

export const Logo = () => {
	const { t } = useTranslation("ui")

	return (
		<Link to="/" className={styles.logoContainer}>
			<img
				src={logo}
				alt={t("header_logo_alt")}
				className={styles.logo}
			/>
		</Link>
	)
}
