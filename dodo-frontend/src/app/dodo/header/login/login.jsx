import React from "react"
import { LoginRegister } from './login-register'
import { UserButton } from './user-button'

import styles from './login.module.scss'

export const Login = ({ isLoggedIn }) => {
	return (
		<div className={styles.login}>
			{isLoggedIn ?
				<UserButton/> :
				<LoginRegister/>
			}
		</div>
	)
}

