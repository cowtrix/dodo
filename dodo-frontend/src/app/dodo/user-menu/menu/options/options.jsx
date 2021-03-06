import React from 'react'
import PropTypes from 'prop-types'
import styles from './options.module.scss'
import { Link } from 'react-router-dom'

import { MY_REBELLION_ROUTE } from '../../../routes/user-routes/my-rebellion'
import { SETTINGS_ROUTE } from '../../../routes/user-routes'

export const Options = ({ logout }) =>
	<ul className={styles.options}>
		<li>
			<Link to={MY_REBELLION_ROUTE}>My Subscriptions</Link>
		</li>
		<li>
			<Link to={SETTINGS_ROUTE}>Profile Settings</Link>
		</li>
		<li>
			<Link onClick={() => logout()} to="/">Logout</Link>
		</li>
	</ul>

Options.propTypes = {
	logout: PropTypes.func,
}
