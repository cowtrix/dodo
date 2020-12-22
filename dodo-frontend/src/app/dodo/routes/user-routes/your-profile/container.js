import { connect } from 'react-redux'
import { YourProfile } from './your-profile'
import { username, name, email, fetchingUser, guid, emailConfirmed, isUpdating, updateError } from 'app/domain/user/selectors'
import { updateDetails, resendVerificationEmail } from 'app/domain/user/actions'

const mapStateToProps = state => ({
	currentUsername: username(state),
	currentName: name(state),
	currentEmail: email(state),
	fetchingUser: fetchingUser(state),
	guid: guid(state),
	isConfirmed: emailConfirmed(state),
	isUpdating: isUpdating(state),
	updateError: updateError(state),
})

const mapDispatchToProps = dispatch => ({
	updateDetails: (slug, name, email, guid) => {
		return updateDetails(dispatch, guid, {slug, name, personalData: { email }})
	},
	resendVerificationEmail: () => resendVerificationEmail(dispatch)
})

export const YourProfileConnected = connect(mapStateToProps, mapDispatchToProps)(YourProfile)
