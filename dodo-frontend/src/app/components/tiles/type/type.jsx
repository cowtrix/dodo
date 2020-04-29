import React from "react"
import PropTypes from "prop-types"
import styles from "./type.module.scss"
import library from "./library"

export const Type = ({ type }) => {
	const eventType = library[type]
	const style = {
		backgroundColor: eventType.color
	}
	return (
		<div className={styles.tile} style={style}>
			<div>{eventType.id}</div>
		</div>
	)
}

Type.propTypes = {
	type: PropTypes.string
}
