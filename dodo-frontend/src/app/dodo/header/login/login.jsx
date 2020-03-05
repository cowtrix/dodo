import React from 'react'
import PropTypes from 'prop-types'
import styles from './login.module.scss'

const loginText = "SIGN IN"

export const Login = ({}) =>
	<div className={styles.login}>
		{loginText}
	</div>

Login.propTypes = {

}