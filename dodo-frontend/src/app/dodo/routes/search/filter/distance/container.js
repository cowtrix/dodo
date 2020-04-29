import { connect } from "react-redux"
import { Distance } from "./distance"

import { selectors, actions } from "app/domain/search"

const mapStateToProps = state => ({
	latlong: selectors.latlong(state),
	distance: selectors.distance(state)
})

const mapDispatchToProps = dispatch => ({
	updateDistance: (distance, latlong) =>
		actions.searchGet(dispatch, { distance, latlong })
})

export const DistanceConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Distance)
