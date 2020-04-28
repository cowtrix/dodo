import React from "react"
import { useTranslation } from "react-i18next"

import { Title } from "app/components/footer"
import styles from "./copyright.module.scss"

export const Copyright = () => {
	const { t } = useTranslation("ui")

	return (
		<div className={styles.copyright}>
			<Title title={t("copyright_info_title")} />
			<div>{t("copyright_info_copy")}</div>
		</div>
	)
}
