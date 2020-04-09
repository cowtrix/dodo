import React from "react"
import PropTypes from "prop-types"

import styles from "./content.module.scss"

export const Content = ({ content }) => (
	<div className={styles.content}>{content}</div>
)

Content.propTypes = {
	content: PropTypes.node
}
