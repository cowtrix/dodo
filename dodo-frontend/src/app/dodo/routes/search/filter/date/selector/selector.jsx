import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./selector.module.scss"

const generateList = startDay => {
	const today = new Date()
	return [
		{
			label: "1 day",
			value: {
				withinStartDate: startDay,
				withinEndDate: today.setDate(today.getDate() - 30)
			}
		},
		{
			label: "7 days",
			value: {
				withinStartDate: startDay,
				withinEndDate: today.setDate(today.getDate() - 21)
			}
		},
		{
			label: "30 days",
			value: {
				withinStartDate: startDay,
				withinEndDate: today.setDate(today.getDate())
			}
		},
		{
			label: "90 days",
			value: {
				withinStartDate: startDay,
				withinEndDate: today.setDate(today.getDate() + 60)
			}
		}
	]
}

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
	withinStartDate: PropTypes.string,
	withinEndDate: PropTypes.string,
	placeholder: PropTypes.string,
	updateDate: PropTypes.func
}
