import { connect } from 'react-redux'
import { Register } from './register'
import { selectors as resourceSelectors } from "app/domain/resources"
import { actions, selectors } from 'app/domain/user/index'

const { username, registeringUser, registerError } = selectors
const { registerUser } = actions
const { privacyPolicy } = resourceSelectors

const mapStateToProps = state => ({
	isLoggedIn: username(state),
	registeringUser: registeringUser(state),
	error: registerError(state),
	privacyPolicy: privacyPolicy(state)
})

const mapDispatchToProps = dispatch => ({
	register: (details) => () => registerUser(dispatch, details)
})

export const RegisterConnected = connect(mapStateToProps, mapDispatchToProps)(Register)
