import React from "react"
import PropTypes from "prop-types"
import { Overlay } from "../overlay"

import styles from "./loader.module.scss"

import xrLogo from "static/xr-logo.svg"

const loaderAlt = "Extiction Rebellion loading spinner"

export const Loader = ({ display }) => (
	<Overlay
		display={display}
		content={<img src={xrLogo} alt={loaderAlt} className={styles.loader} />}
	/>
)

Loader.propTypes = {
	display: PropTypes.bool
}
