import { RESOURCE_GET, RESOURCES_GET, RESOURCE_JOIN } from "./action-types"
import { apiAction } from "../factories/api-action"
import { API_URL } from "../urls"
import { actionTypes as searchActionTypes } from '../search'


export const resourceGet = (dispatch, resourceType, resourceId, cb) =>
	apiAction(dispatch, RESOURCE_GET, API_URL + resourceType + "/" + resourceId, cb)

const setSearchEvents =  dispatch => async payload => {
	dispatch({
		type: searchActionTypes.SEARCH_FILTER_TYPES,
		payload: payload.resourceTypes,
	})
}

export const eventTypesGet = (dispatch) =>
	apiAction(dispatch, RESOURCES_GET, API_URL, setSearchEvents(dispatch))

export const joinResource = (dispatch, resourceType, resourceId, cb) =>
	apiAction(dispatch, RESOURCE_JOIN, API_URL + resourceType + "/" + resourceId + '/join', cb, false, 'post')
