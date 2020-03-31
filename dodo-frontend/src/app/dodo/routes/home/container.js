import { connect } from 'react-redux'
import { Home } from './home'
import { rebellions as rebellionDomain } from 'app/domain'

const { allRebellionsGet } = rebellionDomain.actions


const mapDispatchToProps = dispatch => ({
	getRebellions: () => allRebellionsGet(dispatch)
})

export const HomeConnected = connect(null, mapDispatchToProps)(Home)
