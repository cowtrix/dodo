import { connect } from 'react-redux'
import { Register } from './register'
import { registerUser } from 'app/domain/user/actions'

const mapStateToProps = state => ({

})

const mapDispatchToProps = dispatch => ({
	register: (details) => () => registerUser(dispatch, details)
})

export const RegisterConnected = connect(mapStateToProps, mapDispatchToProps)(Register)
