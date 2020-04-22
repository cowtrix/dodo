import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"

import styles from "./search-bar.module.scss"
import { useTranslation } from "react-i18next"

export const SearchBar = ({ searchValues }) => {
	const { t } = useTranslation("ui")

	return (
		<Select
			className={styles.searchBar}
			placeholder={t("header_select_placeholder")}
			noOptionsMessage={({ inputValue }) =>
				inputValue
					? t("header_select_no_results__for", { inputValue })
					: t("header_select_no_results")
			}
		/>
	)
}

SearchBar.propTypes = {
	searchValues: PropTypes.array
}
