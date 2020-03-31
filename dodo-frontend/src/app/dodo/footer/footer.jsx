import React from 'react'
import { Links } from './links'
import styles from './footer.module.scss'
import { Copyright } from "./copyright"
import { Social } from './social'

export const Footer = () =>
	<div className={styles.footer}>
		<Links/>
		<Copyright/>
    <Social/>
	</div>