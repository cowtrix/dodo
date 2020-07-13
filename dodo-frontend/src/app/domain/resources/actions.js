import { RESOURCE_GET, RESOURCE_NOTIFICATIONS_GET, RESOURCES_GET } from "./action-types"
import { apiAction } from "../factories/api-action"
import { API_URL } from "../urls"
import { actionTypes as searchActionTypes } from '../search'


export const resourceGet = (dispatch, eventType, event, cb) =>
	apiAction(dispatch, RESOURCE_GET, API_URL + eventType + "/" + event, cb)

const setSearchEvents =  dispatch => async payload => {
	dispatch({
		type: searchActionTypes.SEARCH_FILTER_TYPES,
		payload: payload.resourceTypes,
	})
}

export const eventTypesGet = (dispatch) =>
	apiAction(dispatch, RESOURCES_GET, API_URL, setSearchEvents(dispatch))

export const notificationsGet = (dispatch, eventType, event, cb) =>
	apiAction(dispatch, RESOURCE_NOTIFICATIONS_GET, API_URL + eventType + "/notifications/" + event, cb)
