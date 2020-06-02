import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./distance.module.scss"
import { formatEvents } from "./services"

const list = ["1", "10", "100", "1000", "10000"]
const placeholder = "Distance..."

export const Distance = ({ searchParams, search }) => {
	const { distance } = searchParams

	return (
		<Select
			placeholder={placeholder}
			defaultValue={distance}
			options={formatEvents(list)}
			className={styles.selector}
			onChange={value => search({ ...searchParams, distance: value.value })}
		/>
	)
}



Distance.propTypes = {
	searchParams: PropTypes.object,
	search: PropTypes.func
}
