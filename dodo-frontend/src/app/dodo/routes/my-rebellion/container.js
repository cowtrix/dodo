import { connect } from 'react-redux'
import { MyRebellion } from './my-rebellion'
import { username, name, email, fetchingUser } from 'app/domain/user/selectors'
import { updateDetails } from 'app/domain/user/actions'

const mapStateToProps = state => ({
	currentUsername: username(state),
	currentName: name(state),
	currentEmail: email(state),
	fetchingUser: fetchingUser(state)
})

const mapDispatchToProps = dispatch => ({
	updateDetails: (username, name, email) => () => updateDetails(dispatch, { username, name, email })
})

export const MyRebellionConnected = connect(mapStateToProps, mapDispatchToProps)(MyRebellion)
