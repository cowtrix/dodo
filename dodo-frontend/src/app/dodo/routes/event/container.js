import { connect } from "react-redux"
import { Event } from "./event"

import { event } from "app/domain"

const mapStateToProps = state => ({
	event: event.selectors.currentEvent(state),
	isLoading: event.selectors.eventLoading(state)
})

const mapDispatchToProps = dispatch => ({
	getEvent: (eventType, eventID) =>
		event.actions.eventGet(dispatch, eventType, eventID)
})

export const EventConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Event)
