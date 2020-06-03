import { connect } from "react-redux"
import { Event } from "./event"

import { event } from "app/domain"
import { actions, selectors } from 'app/dodo/redux'

const { centerMap } = selectors
const { setCenterMap } = actions

const mapStateToProps = state => ({
	centerMap: centerMap(state),
	event: event.selectors.currentEvent(state),
	isLoading: event.selectors.eventLoading(state)
})

const mapDispatchToProps = dispatch => ({
	setCenterMap: (centerMap) => dispatch(setCenterMap(centerMap)),
	getEvent: (eventType, eventID) =>
		event.actions.eventGet(dispatch, eventType, eventID)
})

export const EventConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Event)
