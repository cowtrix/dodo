import React from "react"
import PropTypes from "prop-types"
import styles from "./type.module.scss"

export const Type = () => {
	return (
		<div className={styles.tile} >
			<div></div>
		</div>
	)
}

Type.propTypes = {
	type: PropTypes.string
}
