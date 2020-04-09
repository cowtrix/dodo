import React from "react"
import PropTypes from "prop-types"
import styles from "./title.module.scss"

export const Title = ({ title, location }) => (
	<div className={styles.titleContainer}>
		<div className={styles.title}>{title}</div>
		<div className={styles.location}>{location}</div>
	</div>
)

Title.propTypes = {
	title: PropTypes.string,
	location: PropTypes.string
}
