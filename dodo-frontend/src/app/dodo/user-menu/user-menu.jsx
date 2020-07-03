import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { Overlay } from 'app/components/overlay'
import { Menu } from './menu'
import styles from './user-menu.module.scss'

export const UserMenu = ({ menuOpen, closeMenu }) =>
	<Fragment>
		<Overlay
				display={menuOpen}
				onClick={() => closeMenu()}
				className={styles.userMenu}
			/>
		<Menu menuOpen={menuOpen}/>
	</Fragment>

UserMenu.propTypes = {
	menuOpen: PropTypes.bool,
	setMenuOpen: PropTypes.func,
}