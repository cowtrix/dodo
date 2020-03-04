import React from 'react'
import logo from './XR-logo.svg'
import styles from './logo.module.scss'

const logoAlt = "Extinction rebellion logo"

export const Logo = () =>
	<img src={logo} alt={logoAlt} className={styles.logo} />