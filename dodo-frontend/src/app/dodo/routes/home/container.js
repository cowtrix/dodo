import { connect } from 'react-redux'
import { Home } from './home'
import { rebellions as rebellionDomain } from 'app/domain'
import { toPairs } from 'ramda'


const { rebellionGet } = rebellionDomain.actions
const { rebellions } = rebellionDomain.selectors

const mapStateToProps = state => ({
	rebellions: rebellions(state)
})

const mapDispatchToProps = dispatch => ({
	getRebellions: (rebellions) => toPairs(rebellions).map(rebellionCode =>
			rebellionGet(dispatch, rebellionCode[1]))
})

export const HomeConnected = connect(mapStateToProps, mapDispatchToProps)(Home)
