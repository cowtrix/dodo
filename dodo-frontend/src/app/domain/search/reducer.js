import {
	SEARCH_FILTER_TYPES,
	SEARCH_GET,
	SEARCH_FILTER_LOCATION,
	SEARCH_FILTER_DISTANCE,
	SEARCH_FILTER_SEARCH
} from "./action-types"

import { SUCCESS } from "../constants"

const initialState = {
	searchResults: [],
	searchParams: {
		types: [],
		latlong: "",
		distance: "",
		search: "",
		page: "",
	}
}

export const reducer = (state = initialState, action) => {
	switch (action.type) {
		case SEARCH_GET + SUCCESS: {
			return {
				...state,
				searchResults: [...action.payload],
			}
		}
		case SEARCH_FILTER_LOCATION: {
			return {
				...state,
				searchParams: {
					...state.searchParams,
					latlong: action.payload
				}
			}
		}
		case SEARCH_FILTER_DISTANCE: {
			return {
				...state,
				searchParams: {
					...state.searchParams,
					distance: action.payload
				}
			}
		}
		case SEARCH_FILTER_SEARCH: {
			return {
				...state,
				searchParams: {
					...state.searchParams,
					search: action.payload
				}
			}
		}
		case SEARCH_FILTER_TYPES: {
			return {
				...state,
				searchParams: {
					...state.searchParams,
					types: action.payload
				}
			}
		}

		default:
			return state
	}
}
