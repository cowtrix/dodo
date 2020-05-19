import { apiReducerFactory } from "./factories"
import { combineReducers } from "redux"

import { actionTypes as event } from "./event"
import { actionTypes as search } from "./search"

export const requestsReducer = combineReducers({
	[event.EVENT_GET]: apiReducerFactory(event.EVENT_GET),
	[search.SEARCH_GET]: apiReducerFactory(search.SEARCH_GET)
})
