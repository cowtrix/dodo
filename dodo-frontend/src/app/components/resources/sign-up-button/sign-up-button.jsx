import React from 'react'
import PropTypes from 'prop-types'
import { Button } from 'app/components'
import styles from './sign-up-button.module.scss'
import { useHistory } from 'react-router-dom'
import { REGISTER_ROUTE } from 'app/dodo/routes/register'


const SIGN_UP_NOW = 'Sign up now'
const T0_RECEIVE_UPDATES = 'To receive updates and news'
const UNSUBSCRIBE = 'Unsubscribe'

const isSubscribedToResource = (memberOf, resourceID) => memberOf.find(resource => resourceID === resource.guid)

export const SignUpButton = ({ updates = false, resourceColor, subscribeResource, resourceID, memberOf = [], resourceType, isLoggedIn = false }) => {
	const { push } = useHistory()
	const isSubscribed = isSubscribedToResource(memberOf, resourceID)
	const subscribe = () => subscribeResource(resourceType, resourceID, 'join')
	const unSubscribe = () => subscribeResource(resourceType, resourceID, 'leave')

	return (
		<div className={styles.wrapper}>
			<Button
				onClick={!isLoggedIn ? () => push(REGISTER_ROUTE) : !isSubscribed ? subscribe : unSubscribe}
				variant="cta"
				style={{backgroundColor: resourceColor}}
			>
				<h1>{!isLoggedIn || !isSubscribed ? SIGN_UP_NOW : UNSUBSCRIBE}</h1>
				{updates ? <h3>{T0_RECEIVE_UPDATES}</h3> : null}
			</Button>
		</div>
	)
}


SignUpButton.propTypes = {
	link: PropTypes.string,
	updates: PropTypes.bool,
}