import React from 'react'
import PropTypes from 'prop-types'
import { Button } from 'app/components'
import styles from './sign-up-button.module.scss'


const SIGN_UP_NOW = 'Sign up now'
const T0_RECEIVE_UPDATES = 'To receive updates and news'
const UNSUBSCRIBE = 'Unsubscribe'

export const SignUpButton = (
	{
		updates = false,
		resourceColor,
		onClick,
		isLoggedIn,
		isSubscribed,
		CTA,
		disable,
		className,
	}) =>
	!disable ?
		<div className={`${styles.wrapper} ${className}`}>
			<Button
				onClick={onClick}
				variant="cta"
				style={{ backgroundColor: resourceColor }}
			>
				{CTA ? <h1>{CTA}</h1> : <h1>{!isLoggedIn || !isSubscribed ? SIGN_UP_NOW : UNSUBSCRIBE}</h1>}
				{updates ? <h3>{T0_RECEIVE_UPDATES}</h3> : null}
			</Button>
		</div> :
		null


SignUpButton.propTypes = {
	resourceColor: PropTypes.string,
	onClick: PropTypes.func,
	isLoggedIn: PropTypes.bool,
	isSubscribed: PropTypes.bool,
	CTA: PropTypes.string,
	disable: PropTypes.bool,
	className: PropTypes.string,
}