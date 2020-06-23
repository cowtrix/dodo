import { connect } from 'react-redux'
import { Login } from './login'

import { selectors } from 'app/domain/user'

const { username } = selectors

const mapStateToProps = state => ({
 isLoggedIn: username(state)
})

const mapDispatchToProps = dispatch => ({

})

export const LoginConnected = connect(mapStateToProps, mapDispatchToProps)(Login)
