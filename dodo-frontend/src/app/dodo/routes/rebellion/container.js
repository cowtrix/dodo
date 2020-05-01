import { connect } from "react-redux"
import { Rebellion } from "./rebellion"

import { rebellions } from "app/domain"

const mapStateToProps = state => ({
	rebellion: rebellions.selectors.currentRebellion(state)
})

const mapDispatchToProps = dispatch => ({
	getRebellion: rebellionID =>
		rebellions.actions.rebellionGet(dispatch, rebellionID)
})

export const RebellionConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Rebellion)
