import { connect } from 'react-redux'
import { MyRebellion } from './my-rebellion'
import { getMyRebellion } from 'app/domain/user/actions'
import { fetchingMyRebellion, fetchingMyRebellionError, myRebellion } from 'app/domain/user/selectors'
import { resourceTypes } from 'app/domain/resources/selectors'


const mapStateToProps = state => ({
	error: fetchingMyRebellionError(state),
	isFetching: fetchingMyRebellion(state),
	myRebellion: myRebellion(state),
	resourceTypes: resourceTypes(state)
})

const mapDispatchToProps = dispatch => ({
	getMyRebellion: () => getMyRebellion(dispatch)
})

export const MyRebellionConnected = connect(mapStateToProps, mapDispatchToProps)(MyRebellion)
