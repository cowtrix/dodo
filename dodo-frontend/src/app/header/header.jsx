import React from 'react'
import logo from './logo-xr.png'
import styles from './header.module.scss'

export const Header = () =>
	<div className={styles.header}>
		<img src={logo} alt="xr logo" />
	</div>

