import { connect } from 'react-redux'
import { Header } from './header'
import { rebellions } from 'app/domain'

const { selectors } = rebellions

const mapStateToProps = state => ({
	rebellionsCount: selectors.rebellions(state).length
})

export const HeaderConnected = connect(mapStateToProps)(Header)
