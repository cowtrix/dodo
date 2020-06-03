import { connect } from "react-redux"
import { Events } from "./events"

import { selectors, actions } from "app/domain/search"
import { selectors as eventSelectors } from 'app/domain/event'

const mapStateToProps = state => ({
	eventTypes: eventSelectors.eventTypes(state),
	searchParams: selectors.searchParams(state)
})

const mapDispatchToProps = dispatch => ({
	search: searchParams => actions.searchGet(dispatch, searchParams)
})

export const EventsConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Events)
