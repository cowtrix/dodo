import { EVENT_GET, EVENT_TYPES_GET } from "./action-types"
import { apiAction } from "../factories/api-action"
import { API_URL } from "../urls"
import { actionTypes as searchActionTypes } from '../search'


export const eventGet = (dispatch, eventType, event) =>
	apiAction(dispatch, EVENT_GET, API_URL + eventType + "/" + event)

const setSearchEvents =  dispatch => async payload => {
	dispatch({
		type: searchActionTypes.SEARCH_FILTER_TYPES,
		payload: payload.resourceTypes,
	})
}

export const eventTypesGet = (dispatch) =>
	apiAction(dispatch, EVENT_TYPES_GET, API_URL, setSearchEvents(dispatch))
