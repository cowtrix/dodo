import { apiReducerFactory } from "./factories"
import { combineReducers } from "redux"

import { actionTypes as event } from "./resources"
import { actionTypes as search } from "./search"
import { actionTypes as user } from "./user"

export const requestsReducer = combineReducers({
	[event.RESOURCE_GET]: apiReducerFactory(event.RESOURCE_GET),
	[event.RESOURCE_NOTIFICATIONS_GET]: apiReducerFactory(event.RESOURCE_NOTIFICATIONS_GET),
	[search.SEARCH_GET]: apiReducerFactory(search.SEARCH_GET),
	[user.LOGIN]: apiReducerFactory(user.LOGIN),
	[user.GET_LOGGED_IN_USER]: apiReducerFactory(user.GET_LOGGED_IN_USER),
	[user.GET_MY_REBELLION]: apiReducerFactory(user.GET_MY_REBELLION),
	[user.REGISTER_USER]: apiReducerFactory(user.REGISTER_USER),
	[user.UPDATE_DETAILS]: apiReducerFactory(user.UPDATE_DETAILS),
	[user.CHANGE_PASSWORD]: apiReducerFactory(user.CHANGE_PASSWORD),
	[event.RESOURCES_GET]: apiReducerFactory(event.RESOURCES_GET)
})
