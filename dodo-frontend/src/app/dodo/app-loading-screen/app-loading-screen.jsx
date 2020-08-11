import React from 'react'
import PropTypes from 'prop-types'
import { Loader } from 'app/components'

import styles from './app-loading-screen.module.scss'

export const AppLoadingScreen = ({ fetchingUser, username }) =>
	<Loader display={!username && fetchingUser} className={styles.loader}/>


AppLoadingScreen.propTypes = {
	username: PropTypes.string,
	fetchingUser: PropTypes.bool,
}