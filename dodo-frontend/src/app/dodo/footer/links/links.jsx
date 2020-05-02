import React from "react"
import { useTranslation } from "react-i18next"

import { Title } from "app/components/footer"
import { Link } from "react-router-dom"
import styles from "./links.module.scss"
import links from "./library"

export const Links = () => {
	const { t } = useTranslation("ui")

	return (
		<div className={styles.linksBox}>
			<Title title={t("footer_menu_links_title")} />
			<ul className={styles.links}>
				{links.map(link => (
					<li key={link.translationKey}>
						<Link to={link.route}>{t(link.translationKey)}</Link>
					</li>
				))}
			</ul>
		</div>
	)
}
