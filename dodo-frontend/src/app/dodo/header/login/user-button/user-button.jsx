import React from 'react'
import PropTypes from 'prop-types'
import userButton from 'static/resources_noun_User_3367734.png'
import styles from './user-button.module.scss'


const USER_BUTTON_ALT = "Button for user menu"

export const UserButton = ({ setMenuOpen, menuOpen }) => (
	<button
		type="button"
		className={styles.userButton}
		onClick={() => setMenuOpen(!menuOpen)}>
		<h4 className={styles.buttonTitle}>PROFILE</h4>
		<img
			src={userButton}
			alt={USER_BUTTON_ALT}
			className={styles.buttonIcon}
		/>
	</button>
);

UserButton.propTypes = {
	setMenuOpen: PropTypes.func,
	menuOpen: PropTypes.bool,
}
