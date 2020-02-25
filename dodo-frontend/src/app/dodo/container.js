import { connect } from 'react-redux'
import { rebellions } from '../domain/index'
import { Dodo } from './dodo'

const { actions } = rebellions

const mapDispatchToProps = dispatch => ({
	allRebellionsGet: () => actions.allRebellionsGet(dispatch)
})

export const DodoConnected = connect(null, mapDispatchToProps)(Dodo)
