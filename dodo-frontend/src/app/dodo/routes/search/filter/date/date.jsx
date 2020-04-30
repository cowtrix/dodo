import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./date.module.scss"

import { generateList } from "./services"

const placeholder = "Date..."

export const Date = ({ withinStartDate, withinEndDate, updateDate }) => (
	<Select
		placeholder={placeholder}
		options={generateList(withinStartDate)}
		className={styles.selector}
		onChange={value =>
			updateDate(value.value.withinStartDate, value.value.withinEndDate)
		}
	/>
)

Date.propTypes = {
	withinStartDate: PropTypes.number,
	withinEndDate: PropTypes.number,
	placeholder: PropTypes.string,
	updateDate: PropTypes.func
}
