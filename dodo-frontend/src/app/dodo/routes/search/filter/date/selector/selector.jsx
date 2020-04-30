import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./selector.module.scss"

import { generateList } from "./services"

export const Selector = ({
	withinStartDate,
	withinEndDate,
	updateDate,
	placeholder
}) => (
	<Select
		placeholder={placeholder}
		options={generateList(withinStartDate)}
		className={styles.selector}
		onChange={value =>
			updateDate(value.value.withinStartDate, value.value.withinEndDate)
		}
	/>
)

Selector.propTypes = {
	withinStartDate: PropTypes.number,
	withinEndDate: PropTypes.number,
	placeholder: PropTypes.string,
	updateDate: PropTypes.func
}
