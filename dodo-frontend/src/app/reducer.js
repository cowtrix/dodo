import { combineReducers } from 'redux'
import { reducerFactory } from "./domain/factories"
import { rebellions, localGroups, search } from './domain'

const { ALL_REBELLIONS_GET } = rebellions.actionTypes
const { ALL_LOCAL_GROUPS_GET } = localGroups.actionTypes
const { SEARCH_GET } = search.actionTypes

export const store = combineReducers({
	domain: combineReducers({
		rebellions: reducerFactory(ALL_REBELLIONS_GET),
		localGroups: reducerFactory(ALL_LOCAL_GROUPS_GET),
		searchResults: reducerFactory(SEARCH_GET)
	})
})