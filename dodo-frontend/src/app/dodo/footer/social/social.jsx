import React from 'react'
import { Title } from "app/components/footer/title"
import { Buttons } from './buttons'
import { Link } from 'react-router-dom'
import styles from './social.module.scss'


import { socialTitle, socialContactCopy } from './constants'


export const Social = () =>
	<div className={styles.social}>
		<Title title={socialTitle}/>
		<Buttons/>
		<Link to="/">{socialContactCopy}</Link>
	</div>
