import React from 'react'
import PropTypes from 'prop-types'
import styles from './options.module.scss'
import { Link } from 'react-router-dom'

import { MY_REBELLION_ROUTE } from '../../../routes/user-routes/my-rebellion'
import { SETTINGS_ROUTE } from '../../../routes/user-routes'

export const Options = ({ logout, closeMenu }) => {
	const handleLogout = () => {
		logout();
		closeMenu();
	}

	return (
		<ul className={styles.options}>
			<li>
				<Link onClick={() => closeMenu()} to={MY_REBELLION_ROUTE}>My Subscriptions</Link>
			</li>
			<li>
				<Link onClick={() => closeMenu()} to={SETTINGS_ROUTE}>Profile Settings</Link>
			</li>
			<li>
				<Link onClick={() => handleLogout()} to="/">Logout</Link>
			</li>
		</ul>
	);
}

Options.propTypes = {
	logout: PropTypes.func,
}
