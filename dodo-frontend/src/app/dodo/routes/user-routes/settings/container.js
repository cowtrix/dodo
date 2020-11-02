import { connect } from 'react-redux'
import { username, name, email, fetchingUser, guid, emailConfirmed } from 'app/domain/user/selectors'
import { updateDetails, resendVerificationEmail } from 'app/domain/user/actions'
import { Settings } from './settings'

const mapStateToProps = state => ({
	currentUsername: username(state),
	currentName: name(state),
	currentEmail: email(state),
	fetchingUser: fetchingUser(state),
	guid: guid(state),
	isConfirmed: emailConfirmed(state)
})

const mapDispatchToProps = dispatch => ({
	updateDetails: (slug, name, email, guid) => () =>
		updateDetails(dispatch, guid, {slug, name, personalData: { email }}),
	resendVerificationEmail: () => resendVerificationEmail(dispatch)
})

export const SettingsConnected = connect(mapStateToProps, mapDispatchToProps)(Settings)
