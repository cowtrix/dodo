import { connect } from 'react-redux'
import { MyRebellion } from './my-rebellion'
import { memberOf } from 'app/domain/user/selectors'
import { resourceTypes } from 'app/domain/resources/selectors'


const mapStateToProps = state => ({
	memberOf: memberOf(state),
	resourceTypes: resourceTypes(state)
})

const mapDispatchToProps = dispatch => ({

})

export const MyRebellionConnected = connect(mapStateToProps, mapDispatchToProps)(MyRebellion)
