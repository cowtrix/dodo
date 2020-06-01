import { connect } from "react-redux"
import { Events } from "./events"

import { selectors, actions } from "app/domain/search"

const mapStateToProps = state => ({
	eventTypes: selectors.events(state),
	searchParams: selectors.searchParams(state)
})

const mapDispatchToProps = dispatch => ({
	searchFilterEvents: events => dispatch(actions.searchFilterEvents(events))
})

export const EventsConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Events)
