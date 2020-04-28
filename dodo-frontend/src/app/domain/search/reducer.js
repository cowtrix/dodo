import {
	SEARCH_FILTER_EVENTS,
	SEARCH_GET,
	SEARCH_FILTER_LOCATION,
	SEARCH_FILTER_DISTANCE
} from "./action-types"
import { SUCCESS } from "../constants"
import { filterByEvent } from "./services"

const initialState = {
	searchResults: [],
	searchResultsFiltered: [],
	events: [],
	latlong: "",
	distance: "10000"
}

export const reducer = (state = initialState, action) => {
	switch (action.type) {
		case SEARCH_GET + SUCCESS: {
			const searchResultsFiltered = state.events.length
				? filterByEvent(action.payload, state.events)
				: state.searchResults
			return {
				...state,
				searchResults: action.payload,
				searchResultsFiltered
			}
		}
		case SEARCH_FILTER_EVENTS: {
			const searchResultsFiltered = action.payload.length
				? filterByEvent(state.searchResults, action.payload)
				: state.searchResults
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

		default:
			return state
	}
}
