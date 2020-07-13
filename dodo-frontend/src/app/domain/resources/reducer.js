import { combineReducers } from 'redux'
import { reducerFactory } from '../factories'
import { RESOURCE_GET, RESOURCE_NOTIFICATIONS_GET, RESOURCES_GET } from './action-types'

export const reducer = combineReducers({
	currentResource: reducerFactory(RESOURCE_GET),
	currentNotifications: reducerFactory(RESOURCE_NOTIFICATIONS_GET, {notifications:[]}),
	resources: reducerFactory(RESOURCES_GET)
})
