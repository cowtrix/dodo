import React from 'react'
import PropTypes from 'prop-types'
import { Overlay } from 'app/components/overlay'
import { Menu } from './menu'
import styles from './user-menu.module.scss'

export const UserMenu = ({ menuOpen, closeMenu }) =>
	<>
		<Overlay
				display={menuOpen}
				onClick={() => closeMenu()}
				className={styles.userMenu}
			/>
		<Menu menuOpen={menuOpen}  closeMenu={closeMenu}/>
	</>

UserMenu.propTypes = {
	menuOpen: PropTypes.bool,
	setMenuOpen: PropTypes.func,
}