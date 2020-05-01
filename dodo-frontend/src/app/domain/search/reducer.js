import {
	SEARCH_FILTER_EVENTS,
	SEARCH_GET,
	SEARCH_FILTER_LOCATION,
	SEARCH_FILTER_DISTANCE,
	SEARCH_FILTER_DATE
} from "./action-types"
import { SUCCESS } from "../constants"
import { filterByEvent, filterByWithinDate } from "./services"

const today = new Date()

const initialState = {
	searchResults: [],
	searchResultsFiltered: [],
	events: [],
	latlong: "",
	distance: "1000",
	withinStartDate: today.setDate(today.getDate() - 30),
	withinEndDate: today.setDate(today.getDate() + 30)
}

export const reducer = (state = initialState, action) => {
	switch (action.type) {
		case SEARCH_GET + SUCCESS: {
			const searchResultsFilteredByEvent = state.events.length
				? filterByEvent({
						searchResults: action.payload,
						events: state.events
				  })
				: action.payload
			const searchResultsFiltered = filterByWithinDate({
				searchResults: searchResultsFilteredByEvent,
				withinStartDate: state.withinStartDate,
				withinEndDate: state.withinEndDate
			})
			return {
				...state,
				searchResults: action.payload,
				searchResultsFiltered
			}
		}
		case SEARCH_FILTER_EVENTS: {
			const searchResultsFilteredByEvent = action.payload.length
				? filterByEvent({
						searchResults: state.searchResults,
						events: action.payload
				  })
				: state.searchResults
			const searchResultsFiltered = filterByWithinDate({
				searchResults: searchResultsFilteredByEvent,
				withinStartDate: state.withinStartDate,
				withinEndDate: state.withinEndDate
			})
			return {
				...state,
				events: action.payload,
				searchResultsFiltered
			}
		}
		case SEARCH_FILTER_LOCATION: {
			return {
				...state,
				latlong: action.payload
			}
		}
		case SEARCH_FILTER_DISTANCE: {
			return {
				...state,
				distance: action.payload
			}
		}
		case SEARCH_FILTER_DATE: {
			const searchResultsFilteredByEvent = state.events.length
				? filterByEvent({
						searchResults: state.searchResults,
						events: state.events
				  })
				: state.searchResults
			const searchResultsFiltered = filterByWithinDate({
				searchResults: searchResultsFilteredByEvent,
				withinStartDate: action.payload.withinStartDate,
				withinEndDate: action.payload.withinEndDate
			})
			return {
				...state,
				withinStartDate: action.payload.withinStartDate,
				withinEndDate: action.payload.withinEndDate,
				searchResultsFiltered
			}
		}

		default:
			return state
	}
}
