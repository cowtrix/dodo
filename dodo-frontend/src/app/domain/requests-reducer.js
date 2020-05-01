import { apiReducerFactory } from "./factories"
import { combineReducers } from "redux"

import { actionTypes as localGroups } from "./local-groups"
import { actionTypes as rebellions } from "./rebellions"
import { actionTypes as search } from "./search"

export const requestsReducer = combineReducers({
	[localGroups.ALL_LOCAL_GROUPS_GET]: apiReducerFactory(
		localGroups.ALL_LOCAL_GROUPS_GET
	),
	[rebellions.ALL_REBELLIONS_GET]: apiReducerFactory(
		rebellions.ALL_REBELLIONS_GET
	),
	[search.SEARCH_GET]: apiReducerFactory(search.SEARCH_GET)
})
