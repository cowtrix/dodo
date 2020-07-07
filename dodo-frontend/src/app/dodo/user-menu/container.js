import { connect } from 'react-redux'
import { UserMenu } from './user-menu'
import { menuOpen } from '../redux/selectors'
import { setMenuOpen } from '../redux/actions'

const mapStateToProps = state => ({
	menuOpen: menuOpen(state)
})

const mapDispatchToProps = dispatch => ({
	closeMenu: () => dispatch(setMenuOpen(false))
})

export const UserMenuConnected = connect(mapStateToProps, mapDispatchToProps)(UserMenu)
