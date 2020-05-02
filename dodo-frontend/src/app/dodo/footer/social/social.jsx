import React from "react"
import { useTranslation } from "react-i18next"

import { Title } from "app/components/footer/title"
import { Buttons } from "./buttons"
import { Link } from "react-router-dom"
import styles from "./social.module.scss"

export const Social = () => {
	const { t } = useTranslation("ui")

	return (
		<div className={styles.social}>
			<Title title={t("social_title")} />
			<Buttons />
			<Link to="/">{t("social_contact_copy")}</Link>
		</div>
	)
}
