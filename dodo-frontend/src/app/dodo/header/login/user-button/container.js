import { connect } from 'react-redux'
import { UserButton } from './user-button'
import { actions, selectors } from 'app/dodo/redux'

const mapStateToProps = state => ({
    menuOpen: selectors.menuOpen(state)
})

const mapDispatchToProps = dispatch => ({
	setMenuOpen: (toggleMenu) => dispatch(actions.setMenuOpen(toggleMenu))
})

export const UserButtonConnected = connect(mapStateToProps, mapDispatchToProps)(UserButton)
