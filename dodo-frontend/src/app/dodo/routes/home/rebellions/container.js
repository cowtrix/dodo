import { connect } from 'react-redux'
import { Rebellions } from './rebellions'
import { rebellions } from 'app/domain'

const { selectors } = rebellions

const mapStateToProps = state => ({
	rebellions: selectors.rebellions(state)
})

export const RebellionsConnected = connect(mapStateToProps)(Rebellions)
