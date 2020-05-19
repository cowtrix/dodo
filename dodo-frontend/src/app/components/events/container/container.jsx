import React from "react"
import PropTypes from "prop-types"
import styles from "./container.module.scss"

export const Container = ({ content }) => (
	<div className={styles.wrapper}>
		<div className={styles.eventListContainer}>{content}</div>
	</div>
)

Container.propTypes = {
	content: PropTypes.node
}
