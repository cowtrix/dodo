import { connect } from 'react-redux'
import { ResetPassword } from './reset-password'
import { resetPassword } from 'app/domain/user/actions'

const mapStateToProps = state => ({

})

const mapDispatchToProps = dispatch => ({
	resetPassword: (email, cb) => resetPassword(dispatch, email, cb)
})

export const ResetPasswordConnected = connect(mapStateToProps, mapDispatchToProps)(ResetPassword)
