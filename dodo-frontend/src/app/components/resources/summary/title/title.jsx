import React from "react"
import PropTypes from "prop-types"
import styles from "./title.module.scss"

export const Title = ({ title, parent }) =>
	<div className={styles.titleContainer}>
		<h2 className={styles.title}>{title}</h2>
		<h4 className={styles.location}>{parent}</h4>
	</div>

Title.propTypes = {
	title: PropTypes.string,
	parent: PropTypes.string
}
