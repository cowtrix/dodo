import { apiReducerFactory } from "./factories"
import { combineReducers } from "redux"

import { actionTypes as event } from "./resources"
import { actionTypes as search } from "./search"

export const requestsReducer = combineReducers({
	[event.RESOURCE_GET]: apiReducerFactory(event.RESOURCE_GET),
	[search.SEARCH_GET]: apiReducerFactory(search.SEARCH_GET)
})
