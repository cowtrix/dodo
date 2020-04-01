import React from 'react'
import logo from './XR-logo.svg'
import styles from './logo.module.scss'
import { Button } from 'app/components/button'

const logoAlt = "Extinction rebellion logo"

export const Logo = () =>
	<Button to="/" className={styles.logoContainer}>
		<img src={logo} alt={logoAlt} className={styles.logo} />
	</Button>