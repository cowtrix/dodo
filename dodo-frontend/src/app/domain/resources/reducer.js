import { combineReducers } from 'redux'
import { reducerFactory } from '../factories'
import { RESOURCE_GET, RESOURCE_NOTIFICATIONS_GET, RESOURCES_GET } from './action-types'
import { REQUEST, SUCCESS } from "../constants"

const defaultNotificationsState = {
	notifications:[],
	nextPageToLoad: 1,
	chunkSize: 5,
	totalCount: 0
}

export const reducer = combineReducers({
	currentResource: reducerFactory(RESOURCE_GET),
	currentNotifications: (state = defaultNotificationsState, action) => {
		switch(action.type){
			case RESOURCE_GET + REQUEST:
				return {
					...defaultNotificationsState
				}
			case RESOURCE_NOTIFICATIONS_GET + SUCCESS:
				return {
					...state,
					...action.payload,
					notifications: [
						...state.notifications,
						...action.payload.notifications
					],
					nextPageToLoad: getNextPageToLoad(state.nextPageToLoad, action.payload.chunkSize, action.payload.totalCount)
				}
			default:
				return state
		}
	},
	resources: reducerFactory(RESOURCES_GET)
})

const getNextPageToLoad = (lastPage, chunkSize, totalCount) => {
	return (lastPage * chunkSize) < totalCount ? lastPage + 1 : false;
}