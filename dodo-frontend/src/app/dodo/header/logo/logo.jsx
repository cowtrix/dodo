import React from "react"
import { Link } from "react-router-dom"
import logo from "./XR-logo.svg"
import styles from "./logo.module.scss"

const logoAlt = "Extinction rebellion logo"

export const Logo = () => (
	<Link to="/" className={styles.logoContainer}>
		<img src={logo} alt={logoAlt} className={styles.logo} />
	</Link>
)
