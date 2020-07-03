import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import styles from './options.module.scss'
import { Link } from 'react-router-dom'


export const Options = ({}) =>
	<ul className={styles.options}>
		<li>
			<Link to="">My Rebellion</Link>
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