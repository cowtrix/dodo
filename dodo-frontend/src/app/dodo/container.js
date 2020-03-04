import { connect } from 'react-redux'
import { rebellions, localGroups } from '../domain/index'
import { Dodo } from './dodo'

const { allRebellionsGet } = rebellions.actions
const { allLocalGroupsGet } = localGroups.actions

const mapDispatchToProps = dispatch => ({
	startup: () => {
		allRebellionsGet(dispatch)
		allLocalGroupsGet(dispatch)
	}
})

export const DodoConnected = connect(null, mapDispatchToProps)(Dodo)
