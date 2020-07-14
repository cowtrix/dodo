import React from 'react'
import PropTypes from 'prop-types'
import styles from './options.module.scss'
import { Link } from 'react-router-dom'

import { YOUR_PROFILE_ROUTE } from '../../../routes/your-profile'
import { MY_REBELLION_ROUTE } from '../../../routes/my-rebellion'

export const Options = ({ logout }) =>
	<ul className={styles.options}>
		<li>
			<Link to={MY_REBELLION_ROUTE}>My Rebellion</Link>
		</li>
		<li>
			<Link to={YOUR_PROFILE_ROUTE}>Edit Profile</Link>
		</li>
		<li>
			<Link to="">Settings</Link>
		</li>
		<li>
			<Link onClick={() => logout()}>Logout</Link>
		</li>
	</ul>

Options.propTypes = {
	logout: PropTypes.func,
}