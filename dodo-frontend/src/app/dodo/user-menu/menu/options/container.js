import { connect } from 'react-redux'
import { Options } from './options'
import { logUserOut } from 'app/domain/user/actions'

const mapStateToProps = state => ({

})

const mapDispatchToProps = dispatch => ({
	logout: () => logUserOut(dispatch)
})

export const OptionsConnected = connect(mapStateToProps, mapDispatchToProps)(Options)

