import { combineReducers } from "redux"
import { reducerFactory } from "./domain/factories"
import { search, requestsReducer, event } from "./domain"

const { EVENT_GET } = event.actionTypes

const searchReducer = search.reducer

export const store = combineReducers({
	domain: combineReducers({
		currentEvent: reducerFactory(EVENT_GET),
		search: searchReducer,
		requests: requestsReducer
	})
})
