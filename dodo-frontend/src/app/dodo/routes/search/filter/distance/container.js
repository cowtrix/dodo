import { connect } from "react-redux"
import { Distance } from "./distance"

import { selectors, actions } from "app/domain/search"

const mapStateToProps = state => ({
	searchParams: selectors.searchParams(state),
})

const mapDispatchToProps = dispatch => ({
	search: (searchParams) =>
		actions.searchGet(dispatch, searchParams)
})

export const DistanceConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Distance)
