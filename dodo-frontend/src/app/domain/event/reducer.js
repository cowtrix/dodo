import { combineReducers } from 'redux'
import { reducerFactory } from '../factories'
import { EVENT_GET, EVENT_TYPES_GET } from './action-types'

export const reducer = combineReducers({
	currentEvent: reducerFactory(EVENT_GET),
	resourceTypes: reducerFactory(EVENT_TYPES_GET)
})