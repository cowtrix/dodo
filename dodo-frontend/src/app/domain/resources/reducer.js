import { combineReducers } from 'redux'
import { reducerFactory } from '../factories'
import { RESOURCE_GET, RESOURCES_GET } from './action-types'

export const reducer = combineReducers({
	currentResource: reducerFactory(RESOURCE_GET),
	resources: reducerFactory(RESOURCES_GET)
})