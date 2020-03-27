import React from 'react'
import styles from './header.module.scss'
import { Logo } from './logo'
import { SearchBar } from './search-bar'
import { Login } from './login'


export const Header = () =>
	<div className={styles.wrapper}>
		<div className={styles.header}>
			<div className={styles.headerLeft}>
				<Logo/>
				<SearchBar/>
			</div>
			<Login/>
		</div>
	</div>

