import React from "react"
import PropTypes from "prop-types"
import Select from "react-select"
import styles from "./events.module.scss"

const placeholder = "Event types..."

export const Events = ({ resourceTypes, searchParams, search }) => {
	return (
		<Select
			placeholder={placeholder}
			isMulti
			value={searchParams.types}
			options={resourceTypes}
			className={styles.selector}
			onChange={value => search({ ...searchParams, types: value && value.length ? value : [], search: "" })
			}
		/>
	)
}

Events.propTypes = {
	resourceTypes: PropTypes.array,
	searchParams: PropTypes.object,
	search: PropTypes.func
}
