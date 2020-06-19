import React from "react"
import PropTypes from "prop-types"
import styles from "./title.module.scss"

export const Title = ({ title, parent }) =>
	<div className={styles.titleContainer}>
		<h2 className={styles.title}>{title}</h2>
		{parent ? <h4 className={styles.location}>{parent.name}</h4> : null}
	</div>

Title.propTypes = {
	title: PropTypes.string,
	parent: PropTypes.string
}
