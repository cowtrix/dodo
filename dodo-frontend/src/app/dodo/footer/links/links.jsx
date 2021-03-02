import React from "react"
import { useTranslation } from "react-i18next"

import { Title } from "app/components/footer"
import styles from "./links.module.scss"

export const Links = ({ privacyPolicy, rebelAgreement }) => {
	const { t } = useTranslation("ui")
	const links = [
		{
			href: '/rsc/about',
			translationKey: 'footer_menu_link_text_about'
		},
		{
			href: `/rsc/faq`,
			translationKey: 'footer_menu_link_text_faq'
		},
		{
			href: `/${privacyPolicy}`,
			translationKey: 'footer_menu_link_text_privacy_policy'
		},
		{
			href: `/${rebelAgreement}`,
			translationKey: 'footer_menu_link_text_rebel_agreement'
		}
	];

	return (
		<div className={styles.linksBox}>
			<Title title={t("footer_menu_links_title")} />
			<ul className={styles.links}>
				{links.map(link => (
					<li key={link.translationKey}>
						<a href={link.href} target="_blank" rel="noopener noreferrer">
							{t(link.translationKey)}
						</a>
					</li>
				))}
			</ul>
		</div>
	)
}
