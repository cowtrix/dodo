import { SEARCH_FILTER_EVENTS, SEARCH_GET } from "./action-types"
import { SUCCESS } from "../constants"
import { filterByEvent } from "./services"

const initialState = {
	searchResults: [],
	searchResultsFiltered: [],
	events: []
}

export const reducer = (state = initialState, action) => {
	switch (action.type) {
		case SEARCH_GET + SUCCESS: {
			return {
				...state,
				searchResults: action.payload,
				searchResultsFiltered: state.events.length
					? filterByEvent(action.payload, state.events)
					: action.payload
			}
		}
		case SEARCH_FILTER_EVENTS: {
			if (action.payload.length) {
				return {
					...state,
					events: action.payload,
					searchResultsFiltered: filterByEvent(
						state.searchResults,
						action.payload
					)
				}
			}
			return {
				...state,
				events: action.payload,
				searchResultsFiltered: state.searchResults
			}
		}
		default:
			return state
	}
}
