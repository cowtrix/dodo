import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import userButton from 'static/resources_noun_User_3367734.png'
import styles from './user-button.module.scss'


const USER_BUTTON_ALT = "Button for user menu"

export const UserButton = ({ notifications }) =>
	<img src={userButton} alt={USER_BUTTON_ALT} className={styles.userButton} />

UserButton.propTypes = {
	notifications: PropTypes.array,
}