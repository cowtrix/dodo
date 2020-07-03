import { connect } from 'react-redux'
import { Menu } from './menu'
import { username } from 'app/domain/user/selectors'

const mapStateToProps = state => ({
    username: username(state)
})

export const MenuConnected = connect(mapStateToProps)(Menu)

