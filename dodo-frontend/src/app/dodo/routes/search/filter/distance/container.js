import { connect } from "react-redux"
import { Distance } from "./distance"

import { selectors, actions } from "app/domain/search"

const mapStateToProps = state => ({
	currentDistance
})

const mapDispatchToProps = dispatch => ({
	updateDistance: params => search.actions.searchGet(dispatch, params)
})

export const EventsConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Distance)
