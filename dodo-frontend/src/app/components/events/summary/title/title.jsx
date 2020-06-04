import React from "react"
import PropTypes from "prop-types"
import styles from "./title.module.scss"

export const Title = ({ title, location }) =>
	<div className={styles.titleContainer}>
		<h2 className={styles.title}>{title}</h2>
		<h4 className={styles.location}>{location}</h4>
	</div>

Title.propTypes = {
	title: PropTypes.string,
	location: PropTypes.string
}
