import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import styles from './menu.module.scss'
import { Options } from './options'

const SIGN_IN_TEXT = 'Signed in as '

export const Menu = ({ menuOpen, username }) =>
	<div className={`${styles.menu} ${menuOpen ? styles.open : ''}`}>
		<div className={styles.user}>
			{SIGN_IN_TEXT}{username}
		</div>
		<Options/>
	</div>

Menu.propTypes = {
	menuOpen: PropTypes.bool,
	username: PropTypes.string,
}