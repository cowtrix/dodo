import { connect } from 'react-redux'
import { Routes } from './routes'
import { setMenuOpen } from 'app/dodo/redux/actions'


const mapStateToProps = state => ({

})

const mapDispatchToProps = dispatch => ({
	closeMenu: () => dispatch(setMenuOpen(false))
})

export const RoutesConnected = connect(mapStateToProps, mapDispatchToProps)(Routes)
