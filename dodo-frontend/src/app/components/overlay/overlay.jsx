import React from "react"
import PropTypes from "prop-types"

import styles from "./overlay.module.scss"

export const Overlay = ({ display, content, onClick, className }) =>
	display ? <div className={`${styles.overlay} ${className}`} onClick={onClick}>{content}</div> : null

Overlay.propTypes = {
	display: PropTypes.bool,
	content: PropTypes.node,
	onClick: PropTypes.func,
}
