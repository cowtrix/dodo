import React from "react"
import PropTypes from "prop-types"

import styles from "./overlay.module.scss"

export const Overlay = ({ display, content }) =>
	display ? <div className={styles.overlay}>{content}</div> : null

Overlay.propTypes = {
	display: PropTypes.bool,
	content: PropTypes.node
}
