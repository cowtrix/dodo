import React from 'react'
import styles from './header.module.scss'
// import header from './home-header.png'
// TODO header image not available
const header = '';

const headerAlt = "XR home header image - protests in the streets"


export const Header = () =>
	<div className={styles.header}>
		<img src={header} alt={headerAlt} className={styles.headerImage} />
	</div>
