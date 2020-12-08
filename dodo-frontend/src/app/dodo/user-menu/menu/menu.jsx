import React from 'react'
import PropTypes from 'prop-types'
import styles from './menu.module.scss'
import { Options } from './options'

const SIGN_IN_TEXT = 'Signed in as '
const PRIVACY_POLICY = 'Privacy Policy'

export const Menu = ({ menuOpen, username, logout }) =>
	<div className={`${styles.menu} ${menuOpen ? styles.open : ''}`}>
		<div className={styles.user}>
			{SIGN_IN_TEXT}{username}
		</div>
		<Options logout={logout} />
		<div className={styles.user}>
			<a href="privacypolicy">{PRIVACY_POLICY}</a>
		</div>
	</div>

Menu.propTypes = {
	menuOpen: PropTypes.bool,
	username: PropTypes.string,
}
