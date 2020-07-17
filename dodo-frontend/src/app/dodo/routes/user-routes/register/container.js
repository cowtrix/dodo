import { connect } from 'react-redux'
import { Register } from './register'
import { actions, selectors } from 'app/domain/user/index'

const { username } = selectors
const { registerUser } = actions

const mapStateToProps = state => ({
	isLoggedIn: username(state),
})

const mapDispatchToProps = dispatch => ({
	register: (details) => () => registerUser(dispatch, details)
})

export const RegisterConnected = connect(mapStateToProps, mapDispatchToProps)(Register)
