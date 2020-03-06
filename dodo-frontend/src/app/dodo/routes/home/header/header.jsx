import React, { Fragment } from 'react'
import header from './home-header.png'
import styles from './header.module.scss'


const headerAlt = "XR home header image - protests in the streets"

export const Header = () =>
	<div className={styles.header}>
		<img src={header} alt={headerAlt} className={styles.headerImage} />
	</div>
