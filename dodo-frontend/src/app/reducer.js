import { combineReducers } from "redux"
import { search, requestsReducer, event } from "./domain"

const searchReducer = search.reducer
const eventReducer = event.reducer

export const store = combineReducers({
	domain: combineReducers({
		search: searchReducer,
		requests: requestsReducer,
		events: eventReducer,
	})
})
