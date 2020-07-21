import { connect } from 'react-redux'
import { AppLoadingScreen } from './app-loading-screen'

import { fetchingUser, username } from 'app/domain/user/selectors'

const mapStateToProps = state => ({
	fetchingUser: fetchingUser(state),
	username: username(state),
})

const mapDispatchToProps = dispatch => ({

})

export const AppLoadingScreenConnected = connect(mapStateToProps, mapDispatchToProps)(AppLoadingScreen)
