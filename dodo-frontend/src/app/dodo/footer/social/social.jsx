import React from "react"
import { useTranslation } from "react-i18next"

import { Title } from "app/components/footer/title"
import { Buttons } from "./buttons"
import styles from "./social.module.scss"

export const Social = () => {
	const { t } = useTranslation("ui")

	return (
		<div className={styles.social}>
			<Title title={t("social_title")} />
			<Buttons />
		</div>
	)
}
