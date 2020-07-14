import { connect } from 'react-redux'
import { Menu } from './menu'
import { username } from 'app/domain/user/selectors'
import { logout } from 'app/domain/user/actions'

const mapStateToProps = state => ({
    username: username(state)
})


export const MenuConnected = connect(mapStateToProps)(Menu)

