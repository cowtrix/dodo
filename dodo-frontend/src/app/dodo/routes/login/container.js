import { connect } from 'react-redux'
import { Login } from './login'
import { actions, selectors } from 'app/domain/user'

const { username, error } = selectors
const { login } = actions

const mapStateToProps = state => ({
	isLoggedIn: username(state),
	error: error(state)
})

const mapDispatchToProps = dispatch => ({
	login: (username, password) => () => login(dispatch, username, password)
})

export const LoginConnected = connect(mapStateToProps, mapDispatchToProps)(Login)
