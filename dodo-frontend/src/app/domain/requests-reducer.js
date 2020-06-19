import { apiReducerFactory } from "./factories"
import { combineReducers } from "redux"

import { actionTypes as event } from "./resources"
import { actionTypes as search } from "./search"
import { actionTypes as user } from "./user"

export const requestsReducer = combineReducers({
	[event.RESOURCE_GET]: apiReducerFactory(event.RESOURCE_GET),
	[search.SEARCH_GET]: apiReducerFactory(search.SEARCH_GET),
	[user.LOGIN]: apiReducerFactory(user.LOGIN)
})
