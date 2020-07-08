import React from 'react'
import PropTypes from 'prop-types'
import styles from './options.module.scss'
import { Link } from 'react-router-dom'

import { MY_REBELLION_ROUTE } from '../../../routes/my-rebellion'

export const Options = ({}) =>
	<ul className={styles.options}>
		<li>
			<Link to={MY_REBELLION_ROUTE}>My Rebellion</Link>
		</li>
		<li>
			<Link to="">Edit Profile</Link>
		</li>
		<li>
			<Link to="">Settings</Link>
		</li>
		<li>
			<Link to="">Logout</Link>
		</li>
	</ul>

Options.propTypes = {

}