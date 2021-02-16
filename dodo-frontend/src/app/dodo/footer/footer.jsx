import React from 'react'
import { Links } from './links'
import styles from './footer.module.scss'
import { Copyright } from "./copyright"
import { Social } from './social'

export const Footer = ({ privacyPolicy, rebelAgreement }) =>
	<div className={styles.footerContainer}>
		<div className={styles.footer}>
			<Links {...{ privacyPolicy, rebelAgreement }} />
			<Copyright/>
	    <Social/>
		</div>
	</div>
