import React from 'react'
import PropTypes from 'prop-types'
import { Button } from 'app/components'
import styles from './sign-up-button.module.scss'


const SIGN_UP_NOW = 'Sign up now'
const T0_RECEIVE_UPDATES = 'To receive updates and news'

export const SignUpButton = ({ link, updates, resourceColor }) =>
	<div className={styles.wrapper}>
		<Button to={link} variant="cta" style={{backgroundColor: resourceColor}}>
			<h1>{SIGN_UP_NOW}</h1>
			{updates ? <h3>{T0_RECEIVE_UPDATES}</h3> : null}
		</Button>
	</div>

SignUpButton.propTypes = {
	link: PropTypes.string,
	updates: PropTypes.bool,
}