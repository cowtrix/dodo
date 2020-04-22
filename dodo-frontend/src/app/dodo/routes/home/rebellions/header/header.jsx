import React from "react"
import PropTypes from "prop-types"
import { Link } from "react-router-dom"
import { useTranslation } from "react-i18next"

import { SubHeader, Icon } from "app/components"
import styles from "./header.module.scss"

import { Button } from "app/components/button"

export const Header = ({ rebellionsCount }) => {
	const { t } = useTranslation("ui")

	return (
		<div className={styles.header}>
			<SubHeader
				content={t("rebellions_header_copy", {
					count: rebellionsCount
				})}
			/>
			<Button type="button" as={<Link to="/search" />}>
				{t("location_search_copy")}
				<Icon icon="bullseye" className={styles.searchIcon} size="sm" />
			</Button>
		</div>
	)
}

Header.propTypes = {
	rebellionsCount: PropTypes.number
}
